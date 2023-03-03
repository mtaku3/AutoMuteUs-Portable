using System.Net.NetworkInformation;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reflection;
using AutoMuteUsPortable.Core.Entity.ComputedSimpleSettingsNS;
using AutoMuteUsPortable.Core.Entity.ConfigNS;
using AutoMuteUsPortable.PocketBaseClient;
using AutoMuteUsPortable.Shared.Controller.Executor;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationSSNS;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;
using AutoMuteUsPortable.Shared.Utility;
using McMaster.NETCore.Plugins;
using Open.Collections;
using Serilog;

namespace AutoMuteUsPortable.Core.Controller.ServerConfigurator;

public class ServerConfiguratorController
{
    private readonly Config _config;
    private readonly PocketBaseClientApplication _pocketBaseClientApplication;
    private readonly CompositeDisposable _pluginLoaders = new();

    public ServerConfiguratorController(Config config, PocketBaseClientApplication pocketBaseClientApplication)
    {
        _config = config;
        _pocketBaseClientApplication = pocketBaseClientApplication;
    }

    public OrderedDictionary<ExecutorType, ExecutorControllerBase> executors { get; } = new();

    private bool IsUsingSimpleSettings => _config.serverConfiguration.simpleSettings != null;

    private static PluginLoader CreatePluginLoader(string assemblyPath)
    {
        return PluginLoader.CreateFromAssemblyFile(assemblyPath, true,
            new[] { typeof(ExecutorControllerBase) });
    }

    public async Task Configure(ISubject<ProgressInfo>? progress = null, CancellationToken cancellationToken = default)
    {
        if (IsUsingSimpleSettings) await ConfigureBySimpleSettings(progress, cancellationToken);
        else await ConfigureByAdvancedSettings(progress, cancellationToken);
    }

    private async Task ConfigureBySimpleSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (0 < _pluginLoaders.Count) return;

        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new Dictionary<string, object?>
            {
                { "Finding listenable ports for executors", null },
                {
                    "Load assembly and create instance of ExecutorController",
                    _config.serverConfiguration.simpleSettings!.executorConfigurations
                        .ToDictionary<ExecutorConfigurationSS, string, object?>(
                            x => $"Loading {x.type} Executor", x => new List<string>
                            {
                                $"Checking file integrity of {x.type} Executor",
                                $"Recovering missing files of {x.type} Executor"
                            })
                }
            })
            : null;

        #endregion

        #region Find listenable port for executors

        var portProgress = taskProgress?.GetSubjectProgress();
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnInfoList = ipGlobalProperties.GetActiveTcpListeners().Select(x => x.Port).Distinct().ToList();
        if (tcpConnInfoList == null) throw new InvalidOperationException("Failed to retrieve active tcp listeners");

        var defaultPort = new Dictionary<string, int>
        {
            { "galactus", 5858 },
            { "broker", 8123 },
            { "postgresql", 5432 },
            { "redis", 6379 }
        };
        var portQueue = new Queue<KeyValuePair<string, int>>();
        foreach (var port in defaultPort.OrderBy(x => x.Value)) portQueue.Enqueue(port);
        var determinedPorts = new Dictionary<string, int>();
        var tcpIdx = 0;
        var (currentName, currentPort) = portQueue.Dequeue();
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (++tcpIdx < tcpConnInfoList.Count)
            {
                var endpoint = tcpConnInfoList[tcpIdx];

                if (endpoint < currentPort) continue;
                if (endpoint == currentPort)
                {
                    currentPort++;
                    continue;
                }
            }

            determinedPorts.Add(currentName, currentPort);
            portProgress?.OnNext(new ProgressInfo
            {
                name = "使用可能なポートを検索しています",
                progress = (double)determinedPorts.Count / defaultPort.Count
            });
            if (portQueue.Count == 0) break;

            var (nextName, nextPort) = portQueue.Dequeue();
            if (nextPort <= currentPort)
                nextPort = currentPort + 1;

            (currentName, currentPort) = (nextName, nextPort);
        }

        taskProgress?.NextTask();

        #endregion

        #region Create ComputedSimpleSettings

        var settings = new ComputedSimpleSettings
        {
            discordToken = _config.serverConfiguration.simpleSettings!.discordToken,
            executorConfigurations = _config.serverConfiguration.simpleSettings.executorConfigurations,
            postgresql = _config.serverConfiguration.simpleSettings.postgresql,
            port = new Port
            {
                galactus = determinedPorts["galactus"],
                broker = determinedPorts["broker"],
                postgresql = determinedPorts["postgresql"],
                redis = determinedPorts["redis"]
            }
        };

        #endregion

        #region Load assembly and create instance of ExecutorController

        foreach (var executorConfiguration in settings.executorConfigurations)
        {
            #region Compare checksum

            var checksumProgress = taskProgress?.GetSubjectProgress();
            checksumProgress?.OnNext(new ProgressInfo
            {
                name = string.Format("{0}のファイルの整合性を確認しています", executorConfiguration.type),
                IsIndeterminate = true
            });
            var executor =
                _pocketBaseClientApplication.Data.ExecutorCollection.FirstOrDefault(x =>
                    x.Type.ToString()?.ToLower() == executorConfiguration.type.ToString() &&
                    x.Version == executorConfiguration.version);
            if (executor == null)
                throw new InvalidOperationException(
                    $"{executorConfiguration.type} Executor {executorConfiguration.version} is not found in the database");

            var checksumUrl = Utils.GetChecksum(executor.Checksum);

            if (string.IsNullOrEmpty(checksumUrl))
            {
#if DEBUG
                // Continue without checksum file
                // TODO: log out as DEBUG Level
#else
                throw new InvalidDataException("Checksum cannot be null or empty");
#endif
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var res = await client.GetStringAsync(checksumUrl, cancellationToken);
                    var invalidFiles = Utils.CompareChecksum(executorConfiguration.executorDirectory,
                        Utils.ParseChecksumText(res), cancellationToken);
                    taskProgress?.NextTask();
                    if (0 < invalidFiles.Count)
                    {
                        var downloadProgress = taskProgress?.GetSubjectProgress();
                        await DownloadExecutor(executorConfiguration, downloadProgress, cancellationToken);
                    }
                }
            }

            taskProgress?.NextTask();

            #endregion

            var assemblyPath = Path.Combine(executorConfiguration.executorDirectory, "AutoMuteUsPortable.Executor.dll");
            var pluginLoader = CreatePluginLoader(assemblyPath);
            var assembly = pluginLoader.LoadDefaultAssembly();
            _pluginLoaders.Add(pluginLoader);

            var constructorArgs = new object[]
            {
                settings,
                executorConfiguration
            };
            var executorController = (ExecutorControllerBase?)assembly.CreateInstance(
                "AutoMuteUsPortable.Executor.ExecutorController", false, BindingFlags.CreateInstance, null,
                constructorArgs, null, null);
            if (executorController == null)
                throw new InvalidOperationException(
                    $"Failed to create new instance of {executorConfiguration.type.ToString()} ExecutorController");
            executors.Add(executorConfiguration.type, executorController);
        }

        #endregion
    }

    private async Task ConfigureByAdvancedSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (0 < _pluginLoaders.Count) return;

        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress,
                _config.serverConfiguration.advancedSettings!.ToDictionary<ExecutorConfiguration, string, object?>(x =>
                    $"Loading {x.type} Executor", x => new List<string>
                {
                    $"Checking file integrity of {x.type} Executor",
                    $"Recovering missing files of {x.type} Executor"
                }))
            : null;

        #endregion

        #region Load assembly, create instance of ExecutorController and run

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
        {
            #region Compare checksum

            var checksumProgress = taskProgress?.GetSubjectProgress();
            checksumProgress?.OnNext(new ProgressInfo
            {
                name = string.Format("{0}のファイルの整合性を確認しています", executorConfiguration.type),
                IsIndeterminate = true
            });
            var executor =
                _pocketBaseClientApplication.Data.ExecutorCollection.FirstOrDefault(x =>
                    x.Type.ToString()?.ToLower() == executorConfiguration.type.ToString() &&
                    x.Version == executorConfiguration.version);
            if (executor == null)
                throw new InvalidOperationException(
                    $"{executorConfiguration.type} Executor {executorConfiguration.version} is not found in the database");

            var checksumUrl = Utils.GetChecksum(executor.Checksum);

            if (string.IsNullOrEmpty(checksumUrl))
            {
#if DEBUG
                // Continue without checksum file
                // TODO: log out as DEBUG Level
#else
                throw new InvalidDataException("Checksum cannot be null or empty");
#endif
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var res = await client.GetStringAsync(checksumUrl, cancellationToken);
                    var invalidFiles = Utils.CompareChecksum(executorConfiguration.executorDirectory,
                        Utils.ParseChecksumText(res));
                    taskProgress?.NextTask();
                    if (0 < invalidFiles.Count)
                    {
                        var downloadProgress = taskProgress?.GetSubjectProgress();
                        await DownloadExecutor(executorConfiguration, downloadProgress, cancellationToken);
                    }
                }
            }

            taskProgress?.NextTask();

            #endregion

            var assemblyPath = Path.Combine(executorConfiguration.executorDirectory, "AutoMuteUsPortable.Executor.dll");
            var pluginLoader = CreatePluginLoader(assemblyPath);
            var assembly = pluginLoader.LoadDefaultAssembly();
            _pluginLoaders.Add(pluginLoader);

            var constructorArgs = new object[]
            {
                executorConfiguration
            };
            var executorController = (ExecutorControllerBase?)assembly.CreateInstance(
                "AutoMuteUsPortable.Executor.ExecutorController", false, BindingFlags.CreateInstance, null,
                constructorArgs, null, null);
            if (executorController == null)
                throw new InvalidOperationException(
                    $"Failed to create new instance of {executorConfiguration.type.ToString()} ExecutorController");
            executors.Add(executorConfiguration.type, executorController);
        }

        #endregion
    }

    public async Task Run(ISubject<ProgressInfo>? progress = null, CancellationToken cancellationToken = default)
    {
        if (IsUsingSimpleSettings) await RunBySimpleSettings(progress, cancellationToken);
        else await RunByAdvancedSettings(progress, cancellationToken);
    }

    private async Task RunBySimpleSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new Dictionary<string, object?>
            {
                { "Configure executors", null },
                {
                    "Run each executors",
                    _config.serverConfiguration.simpleSettings!.executorConfigurations
                        .Select(x => $"Run {x.type}").ToList()
                }
            })
            : null;

        #endregion

        #region Configure executors

        var configureProgress = taskProgress?.GetSubjectProgress();
        await ConfigureBySimpleSettings(configureProgress, cancellationToken);
        taskProgress?.NextTask();

        #endregion

        #region Run each executors

        var runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.redis].Run(runProgress, cancellationToken);
        taskProgress?.NextTask();

        runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.postgresql].Run(runProgress, cancellationToken);
        taskProgress?.NextTask();

        runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.galactus].Run(runProgress, cancellationToken);
        taskProgress?.NextTask();

        runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.automuteus].Run(runProgress, cancellationToken);
        taskProgress?.NextTask();

        #endregion
    }

    private async Task RunByAdvancedSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new Dictionary<string, object?>
            {
                ["Configure executors"] = null,
                ["Run each executors"] =
                    _config.serverConfiguration.advancedSettings!.Select(x => $"Run {x.type}").ToList()
            })
            : null;

        #endregion

        #region Configure executors

        var configureProgress = taskProgress?.GetSubjectProgress();
        await ConfigureByAdvancedSettings(configureProgress, cancellationToken);
        taskProgress?.NextTask();

        #endregion

        #region Run each executors

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
        {
            var runProgress = taskProgress?.GetSubjectProgress();
            await executors[executorConfiguration.type].Run(runProgress, cancellationToken);
            taskProgress?.NextTask();
        }

        #endregion
    }

    public async Task GracefullyStop(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (_config.serverConfiguration.IsSimpleSettingsUsed)
            await GracefullyStopBySimpleSettings(progress, cancellationToken);
        else await GracefullyStopByAdvancedSettings(progress, cancellationToken);
    }

    private async Task GracefullyStopBySimpleSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, executors.Reverse().Select(x => $"Stopping {x.Key}").ToList())
            : null;

        #endregion

        #region Stop servers

        foreach (var (_, executor) in executors.Reverse())
        {
            var stopProgress = taskProgress?.GetSubjectProgress();
            await executor.GracefullyStop(stopProgress, cancellationToken);
            taskProgress?.NextTask();
        }

        #endregion

        #region Unload assemblies

        executors.Clear();
        _pluginLoaders.Clear();

        #endregion
    }


    private async Task GracefullyStopByAdvancedSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, executors.Reverse().Select(x => $"Stopping {x.Key}").ToList())
            : null;

        #endregion

        #region Stop servers

        foreach (var (_, executor) in executors.Reverse())
        {
            var stopProgress = taskProgress?.GetSubjectProgress();
            await executor.GracefullyStop(stopProgress, cancellationToken);
            taskProgress?.NextTask();
        }

        #endregion

        #region Unload assemblies

        executors.Clear();
        _pluginLoaders.Clear();

        #endregion
    }

    public async Task ForciblyStop(ISubject<ProgressInfo>? progress = null)
    {
        if (_config.serverConfiguration.IsSimpleSettingsUsed) await ForciblyStopBySimpleSettings(progress);
        else await ForciblyStopByAdvancedSettings(progress);
    }

    private async Task ForciblyStopBySimpleSettings(ISubject<ProgressInfo>? progress = null)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, executors.Reverse().Select(x => $"Stopping {x.Key}").ToList())
            : null;

        #endregion

        #region Stop servers

        foreach (var (_, executor) in executors.Reverse())
        {
            var stopProgress = taskProgress?.GetSubjectProgress();
            await executor.ForciblyStop(stopProgress);
            taskProgress?.NextTask();
        }

        #endregion

        #region Unload assemblies

        executors.Clear();
        _pluginLoaders.Clear();

        #endregion
    }

    private async Task ForciblyStopByAdvancedSettings(ISubject<ProgressInfo>? progress = null)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, executors.Reverse().Select(x => $"Stopping {x.Key}").ToList())
            : null;

        #endregion

        #region Stop servers

        foreach (var (_, executor) in executors.Reverse())
        {
            var stopProgress = taskProgress?.GetSubjectProgress();
            await executor.ForciblyStop(stopProgress);
            taskProgress?.NextTask();
        }

        #endregion

        #region Unload assemblies

        executors.Clear();
        _pluginLoaders.Clear();

        #endregion
    }

    public async Task Restart(ISubject<ProgressInfo>? progress = null, CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new List<string>
            {
                "Stopping",
                "Restarting"
            })
            : null;

        #endregion

        var stopProgress = taskProgress?.GetSubjectProgress();
        await GracefullyStop(stopProgress, cancellationToken);
        taskProgress?.NextTask();

        var runProgress = taskProgress?.GetSubjectProgress();
        await Run(runProgress, cancellationToken);
        taskProgress?.NextTask();
    }

    public async Task Install(ISubject<ProgressInfo>? progress = null, CancellationToken cancellationToken = default)
    {
        if (IsUsingSimpleSettings) await InstallBySimpleSettings(progress, cancellationToken);
        else await InstallByAdvancedSettings(progress, cancellationToken);
    }

    private async Task DownloadExecutor(ExecutorConfigurationBase executorConfiguration,
        ISubject<ProgressInfo>? progress = null, CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new List<string>
            {
                $"Downloading {executorConfiguration.type} Executor",
                $"Extracting {executorConfiguration.type} Executor"
            })
            : null;

        #endregion

        #region Retrieve data from PocketBase

        var executor =
            _pocketBaseClientApplication.Data.ExecutorCollection.FirstOrDefault(x =>
                x.Type.ToString()?.ToLower() == executorConfiguration.type.ToString() &&
                x.Version == executorConfiguration.version);
        if (executor == null)
            throw new InvalidOperationException(
                $"{executorConfiguration.type} Executor {executorConfiguration.version} is not found in the database");
        var downloadUrl = Utils.GetDownloadUrl(executor.DownloadUrl);
        if (string.IsNullOrEmpty(downloadUrl))
            throw new InvalidDataException("DownloadUrl cannot be null or empty");

        Log.Information("Downloading executor {ExecutorType} {ExecutorVersion} from {DownloadUrl}", executor.Type,
            executor.Version, downloadUrl);

        #endregion

        #region Download and extract

        if (!Directory.Exists(executorConfiguration.executorDirectory))
            Directory.CreateDirectory(executorConfiguration.executorDirectory);
        var binaryPath = Path.Combine(executorConfiguration.executorDirectory, Path.GetFileName(downloadUrl));

        var downloadProgress = taskProgress?.GetProgress();
        if (taskProgress?.ActiveLeafTask != null)
            taskProgress.ActiveLeafTask.Name = string.Format("{0}の実行に必要なファイルをダウンロードしています", executor.Type);
        await Utils.DownloadAsync(downloadUrl, binaryPath, downloadProgress, cancellationToken);
        taskProgress?.NextTask();

        var extractProgress = taskProgress?.GetProgress();
        if (taskProgress?.ActiveLeafTask != null)
            taskProgress.ActiveLeafTask.Name = string.Format("{0}の実行に必要なファイルを解凍しています", executor.Type);
        Utils.ExtractZip(binaryPath, extractProgress, cancellationToken);
        taskProgress?.NextTask();

        #endregion
    }

    private async Task InstallBySimpleSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new Dictionary<string, object?>
            {
                ["Configure executors"] = null,
                ["Install executors"] = _config.serverConfiguration.simpleSettings!.executorConfigurations
                    .Select(x => "Installing {x.type}").ToList()
            })
            : null;

        #endregion

        #region Configure executors

        var configureProgress = taskProgress?.GetSubjectProgress();
        await ConfigureBySimpleSettings(configureProgress, cancellationToken);
        taskProgress?.NextTask();

        #endregion

        #region Install each executors

        var installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.redis].Install(executors.ToDictionary(), installProgress, cancellationToken);
        taskProgress?.NextTask();

        installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.postgresql].Install(executors.ToDictionary(), installProgress, cancellationToken);
        taskProgress?.NextTask();

        installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.galactus].Install(executors.ToDictionary(), installProgress, cancellationToken);
        taskProgress?.NextTask();

        installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.automuteus].Install(executors.ToDictionary(), installProgress, cancellationToken);
        taskProgress?.NextTask();

        #endregion

        #region Unload assemblies

        executors.Clear();
        _pluginLoaders.Clear();

        #endregion
    }

    private async Task InstallByAdvancedSettings(ISubject<ProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new Dictionary<string, object?>
            {
                ["Configure executors"] = null,
                ["Install each executors"] = _config.serverConfiguration.advancedSettings!
                    .Select(x => $"Install {x.type}").ToList()
            })
            : null;

        #endregion

        #region Configure executors

        var configureProgress = taskProgress?.GetSubjectProgress();
        await ConfigureByAdvancedSettings(configureProgress, cancellationToken);
        taskProgress?.NextTask();

        #endregion

        #region Install each executors

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
        {
            var installProgress = taskProgress?.GetSubjectProgress();
            await executors[executorConfiguration.type]
                .Install(executors.ToDictionary(), installProgress, cancellationToken);
            taskProgress?.NextTask();
        }

        #endregion

        #region Unload assemblies

        executors.Clear();
        _pluginLoaders.Clear();

        #endregion
    }
}