using System;
using System.IO;
using AutoMuteUsPortable.Core.Infrastructure.Config;
using AutoMuteUsPortable.Logging;
using AutoMuteUsPortable.UI.Setup.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Windowing;
using Serilog;
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

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .CreateLogger();

        var lifetime = new ClassicDesktopStyleApplicationLifetime
        {
            Args = args,
            ShutdownMode = ShutdownMode.OnMainWindowClose
        };
        AppBuilder.Configure(() => App)
            .UsePlatformDetect()
            .LogToSerilog(
                new LoggerConfiguration()
                    .MinimumLevel.Warning()
                    .Enrich.FromLogContext()
                    .WriteTo.Debug(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3} - Avalonia] [{Area}] {Message} ({SourceType} #{SourceHash}){NewLine}{Exception}")
                    .CreateLogger())
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
            try
            {
                lifetime.Start(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error while running Setup UI");
                return;
            }

            configRepository.Load(DefaultConfigPath);
            if (configRepository.Config == null) return;

            App.InitializeMainUI();
            lifetime.MainWindow = UI.Main.Views.MainWindow.Instance;
            try
            {
                lifetime.Start(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error while running Main UI");
            }

            #endregion
        }
        else
        {
            #region Launch Main

            App.InitializeMainUI();
            lifetime.MainWindow = UI.Main.Views.MainWindow.Instance;
            try
            {
                lifetime.Start(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error while running Main UI");
            }

            #endregion
        }
    }
}