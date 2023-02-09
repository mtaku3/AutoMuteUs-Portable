using System.Net.NetworkInformation;
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

namespace AutoMuteUsPortable.Core.Controller.ServerConfigurator;

public class ServerConfiguratorController
{
    private readonly Config _config;
    private readonly PocketBaseClientApplication _pocketBaseClientApplication;

    public ServerConfiguratorController(Config config, PocketBaseClientApplication pocketBaseClientApplication)
    {
        _config = config;
        _pocketBaseClientApplication = pocketBaseClientApplication;
    }

    public Dictionary<ExecutorType, PluginLoader> pluginLoaders { get; } = new();
    public Dictionary<ExecutorType, ExecutorControllerBase> executors { get; } = new();

    private bool IsUsingSimpleSettings => _config.serverConfiguration.simpleSettings != null;

    public async Task Run(ISubject<ProgressInfo>? progress = null)
    {
        if (IsUsingSimpleSettings) await RunBySimpleSettings(progress);
        else await RunByAdvancedSettings(progress);
    }

    private async Task RunBySimpleSettings(ISubject<ProgressInfo>? progress = null)
    {
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
                },
                {
                    "Run each executors",
                    _config.serverConfiguration.simpleSettings!.executorConfigurations
                        .Select(x => $"Running {x.type}").ToList()
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
            if (string.IsNullOrEmpty(checksumUrl)) throw new InvalidDataException("Checksum cannot be null or empty");

            using (var client = new HttpClient())
            {
                var res = await client.GetStringAsync(checksumUrl);
                var invalidFiles = Utils.CompareChecksum(executorConfiguration.executorDirectory,
                    Utils.ParseChecksumText(res));
                taskProgress?.NextTask();
                if (0 < invalidFiles.Count)
                {
                    var downloadProgress = taskProgress?.GetSubjectProgress();
                    await DownloadExecutor(executorConfiguration, downloadProgress);
                }
            }

            taskProgress?.NextTask();

            #endregion

            var assemblyPath = Path.Combine(executorConfiguration.executorDirectory, "AutoMuteUsPortable.Executor.dll");
            var pluginLoader = PluginLoader.CreateFromAssemblyFile(
                assemblyPath, true, new[] { typeof(ExecutorControllerBase) });
            var assembly = pluginLoader.LoadDefaultAssembly();
            pluginLoaders.Add(executorConfiguration.type, pluginLoader);

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

        #region Run each executors

        var runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.redis].Run(runProgress);
        taskProgress?.NextTask();

        runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.postgresql].Run();
        taskProgress?.NextTask();

        runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.galactus].Run();
        taskProgress?.NextTask();

        runProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.automuteus].Run();
        taskProgress?.NextTask();

        #endregion
    }

    private async Task RunByAdvancedSettings(ISubject<ProgressInfo>? progress = null)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress,
                _config.serverConfiguration.advancedSettings!.ToDictionary<ExecutorConfiguration, string, object?>(x =>
                    $"Loading {x.type} Executor", x => new List<string>
                {
                    $"Checking file integrity of {x.type} Executor",
                    $"Recovering missing files of {x.type} Executor",
                    $"Running {x.type}"
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
            if (string.IsNullOrEmpty(checksumUrl)) throw new InvalidDataException("Checksum cannot be null or empty");

            using (var client = new HttpClient())
            {
                var res = await client.GetStringAsync(checksumUrl);
                var invalidFiles = Utils.CompareChecksum(executorConfiguration.executorDirectory,
                    Utils.ParseChecksumText(res));
                taskProgress?.NextTask();
                if (0 < invalidFiles.Count)
                {
                    var downloadProgress = taskProgress?.GetSubjectProgress();
                    await DownloadExecutor(executorConfiguration, downloadProgress);
                }
            }

            taskProgress?.NextTask();

            #endregion

            var assemblyPath = Path.Combine(executorConfiguration.executorDirectory, "AutoMuteUsPortable.Executor.dll");
            var pluginLoader = PluginLoader.CreateFromAssemblyFile(
                assemblyPath, true, new[] { typeof(ExecutorControllerBase) });
            var assembly = pluginLoader.LoadDefaultAssembly();
            pluginLoaders.Add(executorConfiguration.type, pluginLoader);

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

            var runProgress = taskProgress?.GetSubjectProgress();
            await executorController.Run(runProgress);
            taskProgress?.NextTask();
        }

        #endregion
    }

    public async Task Stop(ISubject<ProgressInfo>? progress = null)
    {
        if (IsUsingSimpleSettings) await StopBySimpleSettings(progress);
        else await StopByAdvancedSettings(progress);
    }

    private async Task StopBySimpleSettings(ISubject<ProgressInfo>? progress = null)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new List<string>
            {
                "Stopping automuteus",
                "Stopping postgresql",
                "Stopping redis"
            })
            : null;

        #endregion

        #region Stop servers

        var stopProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.automuteus].Stop(stopProgress);
        taskProgress?.NextTask();

        stopProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.galactus].Stop(stopProgress);
        taskProgress?.NextTask();

        stopProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.postgresql].Stop(stopProgress);
        taskProgress?.NextTask();

        stopProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.redis].Stop(stopProgress);
        taskProgress?.NextTask();

        #endregion

        #region Unload assemblies

        executors.Clear();
        foreach (var (type, pluginLoader) in pluginLoaders)
        {
            pluginLoaders.Remove(type);
            pluginLoader.Dispose();
        }

        #endregion
    }

    private async Task StopByAdvancedSettings(ISubject<ProgressInfo>? progress = null)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress,
                _config.serverConfiguration.advancedSettings!.Select(x => $"Stopping {x.type}").ToList())
            : null;

        #endregion

        #region Stop servers

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
        {
            var stopProgress = taskProgress?.GetSubjectProgress();
            await executors[executorConfiguration.type].Stop(stopProgress);
            taskProgress?.NextTask();
        }

        #endregion

        #region Unload assemblies

        executors.Clear();
        foreach (var (type, pluginLoader) in pluginLoaders)
        {
            pluginLoaders.Remove(type);
            pluginLoader.Dispose();
        }

        #endregion
    }

    public async Task Restart(ISubject<ProgressInfo>? progress = null)
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
        await Stop(stopProgress);
        taskProgress?.NextTask();

        var runProgress = taskProgress?.GetSubjectProgress();
        await Run(runProgress);
        taskProgress?.NextTask();
    }

    public async Task Install(ISubject<ProgressInfo>? progress = null)
    {
        if (IsUsingSimpleSettings) await InstallBySimpleSettings(progress);
        else await InstallByAdvancedSettings(progress);
    }

    private async Task DownloadExecutor(ExecutorConfigurationBase executorConfiguration,
        ISubject<ProgressInfo>? progress = null)
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

        #endregion

        #region Download and extract

        if (!Directory.Exists(executorConfiguration.executorDirectory))
            Directory.CreateDirectory(executorConfiguration.executorDirectory);
        var binaryPath = Path.Combine(executorConfiguration.executorDirectory, Path.GetFileName(downloadUrl));

        var downloadProgress = taskProgress?.GetProgress();
        if (taskProgress?.ActiveLeafTask != null)
            taskProgress.ActiveLeafTask.Name = string.Format("{0}の実行に必要なファイルをダウンロードしています", executor.Type);
        await Utils.DownloadAsync(downloadUrl, binaryPath, downloadProgress);
        taskProgress?.NextTask();

        var extractProgress = taskProgress?.GetProgress();
        if (taskProgress?.ActiveLeafTask != null)
            taskProgress.ActiveLeafTask.Name = string.Format("{0}の実行に必要なファイルを解凍しています", executor.Type);
        Utils.ExtractZip(binaryPath, extractProgress);
        taskProgress?.NextTask();

        #endregion
    }

    private async Task InstallBySimpleSettings(ISubject<ProgressInfo>? progress = null)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress, new Dictionary<string, object?>
            {
                { "Finding listenable ports for executors", null },
                {
                    "Load assembly and create instance of ExecutorController",
                    _config.serverConfiguration.simpleSettings!.executorConfigurations
                        .Select(x => $"Downloading {x.type} Executor").ToList()
                },
                {
                    "Install executors",
                    _config.serverConfiguration.simpleSettings!.executorConfigurations
                        .Select(x => "Installing {x.type}").ToList()
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
            #region Download and extract

            var downloadProgress = taskProgress?.GetSubjectProgress();
            await DownloadExecutor(executorConfiguration, downloadProgress);
            taskProgress?.NextTask();

            #endregion

            var assemblyPath = Path.Combine(executorConfiguration.executorDirectory, "AutoMuteUsPortable.Executor.dll");
            var pluginLoader = PluginLoader.CreateFromAssemblyFile(
                assemblyPath, true, new[] { typeof(ExecutorControllerBase) });
            var assembly = pluginLoader.LoadDefaultAssembly();
            pluginLoaders.Add(executorConfiguration.type, pluginLoader);

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

        #region Install each executors

        var installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.redis].Install(executors, installProgress);
        taskProgress?.NextTask();

        installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.postgresql].Install(executors, installProgress);
        taskProgress?.NextTask();

        installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.galactus].Install(executors, installProgress);
        taskProgress?.NextTask();

        installProgress = taskProgress?.GetSubjectProgress();
        await executors[ExecutorType.automuteus].Install(executors, installProgress);
        taskProgress?.NextTask();

        #endregion

        #region Unload assemblies

        executors.Clear();
        foreach (var (type, pluginLoader) in pluginLoaders)
        {
            pluginLoaders.Remove(type);
            pluginLoader.Dispose();
        }

        #endregion
    }

    private async Task InstallByAdvancedSettings(ISubject<ProgressInfo>? progress = null)
    {
        #region Setup progress

        var taskProgress = progress != null
            ? new TaskProgress(progress,
                _config.serverConfiguration.advancedSettings!.ToDictionary<ExecutorConfiguration, string, object?>(
                    x => $"Installing {x.type}", x => new List<string>
                    {
                        $"Downloading {x.type} Executor",
                        $"Installing {x.type}"
                    }))
            : null;

        #endregion

        #region Load assembly, create instance of ExecutorController and install

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
        {
            taskProgress?.Task!.AddTask("Downloading");
            taskProgress?.Task!.AddTask("Installing");

            var downloadProgress = taskProgress?.GetSubjectProgress();
            await DownloadExecutor(executorConfiguration, downloadProgress);
            taskProgress?.NextTask();

            var assemblyPath = Path.Combine(executorConfiguration.executorDirectory, "AutoMuteUsPortable.Executor.dll");
            var pluginLoader = PluginLoader.CreateFromAssemblyFile(
                assemblyPath, true, new[] { typeof(ExecutorControllerBase) });
            var assembly = pluginLoader.LoadDefaultAssembly();
            pluginLoaders.Add(executorConfiguration.type, pluginLoader);

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

            var installProgress = taskProgress?.GetSubjectProgress();
            await executorController.Install(executors, installProgress);
            taskProgress?.NextTask();
        }

        #endregion

        #region Unload assemblies

        executors.Clear();
        foreach (var (type, pluginLoader) in pluginLoaders)
        {
            pluginLoaders.Remove(type);
            pluginLoader.Dispose();
        }

        #endregion
    }
}