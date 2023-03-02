using System;
using System.IO;
using AutoMuteUsPortable.Core.Infrastructure.Config;
using AutoMuteUsPortable.UI.Setup.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Windowing;
#if DEBUG
using DotNetEnv;
#endif

namespace AutoMuteUsPortable;

internal class Program
{
    private static readonly string DefaultConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoMuteUsPortable.json");

    private static readonly App App = new();

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        Env.TraversePath().Load();
#endif

        var lifetime = new ClassicDesktopStyleApplicationLifetime
        {
            Args = args,
            ShutdownMode = ShutdownMode.OnMainWindowClose
        };
        AppBuilder.Configure(() => App)
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI()
            .UseFAWindowing()
            .SetupWithLifetime(lifetime);

        var configRepository = new ConfigRepository();
        configRepository.Load(DefaultConfigPath);

        if (configRepository.Config == null)
        {
            #region Launch Setup and then Main

            App.InitializeSetupUI();
            lifetime.MainWindow = MainWindow.Instance;
            lifetime.Start(args);

            configRepository.Load(DefaultConfigPath);
            if (configRepository.Config == null) return;

            App.InitializeMainUI();
            lifetime.MainWindow = UI.Main.Views.MainWindow.Instance;
            lifetime.Start(args);

            #endregion
        }
        else
        {
            #region Launch Main

            App.InitializeMainUI();
            lifetime.MainWindow = UI.Main.Views.MainWindow.Instance;
            lifetime.Start(args);

            #endregion
        }
    }
}