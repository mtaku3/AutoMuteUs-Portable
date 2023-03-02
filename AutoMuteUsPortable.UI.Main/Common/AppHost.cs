using System;
using System.IO;
using AutoMuteUsPortable.Core.Controller.ServerConfigurator;
using AutoMuteUsPortable.Core.Infrastructure.Config;
using AutoMuteUsPortable.PocketBaseClient;

namespace AutoMuteUsPortable.UI.Main.Common;

public class AppHost
{
    public static readonly string DefaultConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoMuteUsPortable.json");

    public static readonly string ProcessPath = Environment.ProcessPath!;

    public AppHost()
    {
        ConfigRepository = new ConfigRepository();
        ConfigRepository.Load(DefaultConfigPath);

        PocketBaseClientApplication = new PocketBaseClientApplication();
        ServerConfiguratorController =
            new ServerConfiguratorController(ConfigRepository.Config, PocketBaseClientApplication);

        if (IsSimpleSettings)
            LogPageController = new LogPageController(ConfigRepository.Config.serverConfiguration.simpleSettings!
                .executorConfigurations);
        else
            LogPageController =
                new LogPageController(ConfigRepository.Config.serverConfiguration.advancedSettings!);
    }

    public static AppHost Instance { get; private set; }

    public bool IsSimpleSettings => ConfigRepository.Config.serverConfiguration.IsSimpleSettingsUsed;
    public ConfigRepository ConfigRepository { get; }
    public PocketBaseClientApplication PocketBaseClientApplication { get; }
    public ServerConfiguratorController ServerConfiguratorController { get; }
    public LogPageController LogPageController { get; }

    public static void Initialize()
    {
        if (Instance != null) throw new InvalidOperationException("AppHost is already initialized");

        Instance = new AppHost();
    }
}