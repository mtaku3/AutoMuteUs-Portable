using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Loader;
using AutoMuteUsPortable.Core.Entity.ComputedSimpleSettingsNS;
using AutoMuteUsPortable.Core.Entity.ConfigNS;
using AutoMuteUsPortable.Shared.Controller.Executor;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;

namespace AutoMuteUsPortable.Core.Controller.ServerConfigurator;

public class ServerConfiguratorController
{
    public readonly Config Config;
    public Dictionary<ExecutorType, AssemblyLoadContext> assemblyLoadContexts { get; } = new();
    public Dictionary<ExecutorType, ExecutorControllerBase> executors { get; } = new();

    private bool IsUsingSimpleSettings => Config.serverConfiguration.simpleSettings != null;

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
            discordToken = Config.serverConfiguration.simpleSettings!.discordToken,
            executorConfigurations = Config.serverConfiguration.simpleSettings.executorConfigurations,
            postgresql = Config.serverConfiguration.simpleSettings.postgresql,
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
            var assemblyPath = Path.Combine(Config.installedDirectory, @"Executors\");
            var alc = new AssemblyLoadContext(executorConfiguration.type.ToString(), true);
            var assembly = alc.LoadFromAssemblyPath(assemblyPath);
            assemblyLoadContexts.Add(executorConfiguration.type, alc);

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

        foreach (var executorConfiguration in Config.serverConfiguration.advancedSettings!)
        {
            var assemblyPath = Path.Combine(Config.installedDirectory, @"Executors\");
            var alc = new AssemblyLoadContext(executorConfiguration.type.ToString(), true);
            var assembly = alc.LoadFromAssemblyPath(assemblyPath);
            assemblyLoadContexts.Add(executorConfiguration.type, alc);

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
        foreach (var alc in assemblyLoadContexts.Values) alc.Unload();

        #endregion
    }

    private async Task StopByAdvancedSettings()
    {
        #region Stop servers

        foreach (var executorConfiguration in Config.serverConfiguration.advancedSettings!)
            await executors[executorConfiguration.type].Stop();

        #endregion

        #region Unload assemblies

        executors.Clear();
        foreach (var alc in assemblyLoadContexts.Values) alc.Unload();

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
            discordToken = Config.serverConfiguration.simpleSettings!.discordToken,
            executorConfigurations = Config.serverConfiguration.simpleSettings.executorConfigurations,
            postgresql = Config.serverConfiguration.simpleSettings.postgresql,
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
            var assemblyPath = Path.Combine(Config.installedDirectory, @"Executors\");
            var alc = new AssemblyLoadContext(executorConfiguration.type.ToString(), true);
            var assembly = alc.LoadFromAssemblyPath(assemblyPath);
            assemblyLoadContexts.Add(executorConfiguration.type, alc);

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
        foreach (var alc in assemblyLoadContexts.Values) alc.Unload();

        #endregion
    }

    private async Task InstallByAdvancedSettings()
    {
        #region Load assembly, create instance of ExecutorController and install

        foreach (var executorConfiguration in Config.serverConfiguration.advancedSettings!)
        {
            var assemblyPath = Path.Combine(Config.installedDirectory, @"Executors\");
            var alc = new AssemblyLoadContext(executorConfiguration.type.ToString(), true);
            var assembly = alc.LoadFromAssemblyPath(assemblyPath);
            assemblyLoadContexts.Add(executorConfiguration.type, alc);

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
        foreach (var alc in assemblyLoadContexts.Values) alc.Unload();

        #endregion
    }
}