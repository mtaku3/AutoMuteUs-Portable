using System;
using System.IO;
using AutoMuteUsPortable.Core.Controller.ServerConfigurator;
using AutoMuteUsPortable.Core.Infrastructure.Config;
using AutoMuteUsPortable.PocketBaseClient;
using AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;
using AutoMuteUsPortable.UI.Main;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMuteUsPortable;

internal class Program
{
    public static readonly string DefaultConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoMuteUsPortable.json");

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        #region Find config to launch with

        var configBaseRepository = new ConfigBaseRepository();
        configBaseRepository.LoadOrCreateDefault(DefaultConfigPath);

        var configBase = configBaseRepository.FindUnique(Environment.ProcessPath!);
        if (configBase == null)
            // TODO: show window to select which config user want to launch with
            throw new NotImplementedException();

        var configRepository = new ConfigRepository(configBase.executableFilePath);
        configRepository.Load(DefaultConfigPath);

        var pocketBaseClientApplication = new PocketBaseClientApplication();

        #endregion

        var serverConfigurator =
            new ServerConfiguratorController(configRepository.ActiveConfig, pocketBaseClientApplication);

        #region Dependency Injection - Repository

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IConfigBaseRepository>(configBaseRepository);
        serviceCollection.AddSingleton<IConfigRepository>(configRepository);
        serviceCollection.AddSingleton(pocketBaseClientApplication);

        var service = serviceCollection.BuildServiceProvider();

        #endregion

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}