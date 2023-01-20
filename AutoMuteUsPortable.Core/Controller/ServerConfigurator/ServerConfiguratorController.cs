using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using AutoMuteUsPortable.Core.Entity.ComputedSimpleSettingsNS;
using AutoMuteUsPortable.Core.Entity.ConfigNS;
using AutoMuteUsPortable.PocketBaseClient;
using AutoMuteUsPortable.Shared.Controller.Executor;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using McMaster.NETCore.Plugins;
using Standart.Hash.xxHash;
using Utils = AutoMuteUsPortable.Shared.Utility.Utils;

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

    public async Task Run()
    {
        if (IsUsingSimpleSettings) await RunBySimpleSettings();
        else await RunByAdvancedSettings();
    }

    private async Task RunBySimpleSettings()
    {
        #region Find listenable port for executors

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
            if (portQueue.Count == 0) break;

            var (nextName, nextPort) = portQueue.Dequeue();
            if (nextPort <= currentPort)
                nextPort = currentPort + 1;

            (currentName, currentPort) = (nextName, nextPort);
        }

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
                var invalidFiles = Utils.CompareChecksum(executorConfiguration.binaryDirectory,
                    Utils.ParseChecksumText(res));
                if (0 < invalidFiles.Count)
                    await DownloadExecutor(_config.installedDirectory, executorConfiguration.type.ToString(),
                        executorConfiguration.version);
            }

            #endregion

            var assemblyPath = Path.Combine(_config.installedDirectory,
                $@"Executors\{EncodeExecutorDirectory(executorConfiguration.type.ToString(), executorConfiguration.version)}\AutoMuteUsPortable.Executor.dll");
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

        await executors[ExecutorType.redis].Run();
        await executors[ExecutorType.postgresql].Run();
        await executors[ExecutorType.galactus].Run();
        await executors[ExecutorType.automuteus].Run();

        #endregion
    }

    private async Task RunByAdvancedSettings()
    {
        #region Load assembly, create instance of ExecutorController and run

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
        {
            #region Compare checksum

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
                var invalidFiles = Utils.CompareChecksum(executorConfiguration.binaryDirectory,
                    Utils.ParseChecksumText(res));
                if (0 < invalidFiles.Count)
                    await DownloadExecutor(_config.installedDirectory, executorConfiguration.type.ToString(),
                        executorConfiguration.version);
            }

            #endregion

            var assemblyPath = Path.Combine(_config.installedDirectory,
                $@"Executors\{EncodeExecutorDirectory(executorConfiguration.type.ToString(), executorConfiguration.version)}\AutoMuteUsPortable.Executor.dll");
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

            await executorController.Run();
        }

        #endregion
    }

    public async Task Stop()
    {
        if (IsUsingSimpleSettings) await StopBySimpleSettings();
        else await StopByAdvancedSettings();
    }

    private async Task StopBySimpleSettings()
    {
        #region Stop servers

        await executors[ExecutorType.automuteus].Stop();
        await executors[ExecutorType.galactus].Stop();
        await executors[ExecutorType.postgresql].Stop();
        await executors[ExecutorType.redis].Stop();

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

    private async Task StopByAdvancedSettings()
    {
        #region Stop servers

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
            await executors[executorConfiguration.type].Stop();

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

    public async Task Restart()
    {
        await Stop();
        await Run();
    }

    public async Task Install()
    {
        if (IsUsingSimpleSettings) await InstallBySimpleSettings();
        else await InstallByAdvancedSettings();
    }

    private async Task DownloadExecutor(string installedDirectory, string type, string version)
    {
        #region Retrieve data from PocketBase

        var executor =
            _pocketBaseClientApplication.Data.ExecutorCollection.FirstOrDefault(x =>
                x.Type.ToString()?.ToLower() == type && x.Version == version);
        if (executor == null)
            throw new InvalidOperationException($"{type} Executor {version} is not found in the database");
        var downloadUrl = Utils.GetDownloadUrl(executor.DownloadUrl);
        if (string.IsNullOrEmpty(downloadUrl))
            throw new InvalidDataException("DownloadUrl cannot be null or empty");

        #endregion

        #region Download and extract

        var executorDirectory =
            Path.Combine(installedDirectory, $@"Executors\{EncodeExecutorDirectory(type, version)}");
        if (!Directory.Exists(executorDirectory)) Directory.CreateDirectory(executorDirectory);
        var binaryPath = Path.Combine(executorDirectory,
            Path.GetFileName(downloadUrl));

        await Utils.DownloadAsync(downloadUrl, binaryPath);
        Utils.ExtractZip(binaryPath);

        #endregion
    }

    private string EncodeExecutorDirectory(string type, string version)
    {
        var bytes = Encoding.UTF8.GetBytes($"{type} {version}");
        return xxHash3.ComputeHash(bytes, bytes.Length).ToString("x16");
    }

    private async Task InstallBySimpleSettings()
    {
        #region Find listenable port for executors

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
            if (portQueue.Count == 0) break;

            var (nextName, nextPort) = portQueue.Dequeue();
            if (nextPort <= currentPort)
                nextPort = currentPort + 1;

            (currentName, currentPort) = (nextName, nextPort);
        }

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
            await DownloadExecutor(_config.installedDirectory, executorConfiguration.type.ToString(),
                executorConfiguration.version);

            var assemblyPath = Path.Combine(_config.installedDirectory,
                $@"Executors\{EncodeExecutorDirectory(executorConfiguration.type.ToString(), executorConfiguration.version)}\AutoMuteUsPortable.Executor.dll");
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

        await executors[ExecutorType.redis].Install(executors);
        await executors[ExecutorType.postgresql].Install(executors);
        await executors[ExecutorType.galactus].Install(executors);
        await executors[ExecutorType.automuteus].Install(executors);

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

    private async Task InstallByAdvancedSettings()
    {
        #region Load assembly, create instance of ExecutorController and install

        foreach (var executorConfiguration in _config.serverConfiguration.advancedSettings!)
        {
            await DownloadExecutor(_config.installedDirectory, executorConfiguration.type.ToString(),
                executorConfiguration.version);

            var assemblyPath = Path.Combine(_config.installedDirectory,
                $@"Executors\{EncodeExecutorDirectory(executorConfiguration.type.ToString(), executorConfiguration.version)}\AutoMuteUsPortable.Executor.dll");
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

            await executorController.Install(executors);
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