using System;
using System.Collections.Generic;
using System.IO;
using AutoMuteUsPortable.Core.Infrastructure.Config;
using AutoMuteUsPortable.Logging;
using AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;
using AutoMuteUsPortable.UI.Setup.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Windowing;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Templates;
#if DEBUG
using DotNetEnv;
#endif

namespace AutoMuteUsPortable;

internal class Program
{
    private static readonly string DefaultConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoMuteUsPortable.json");

    private static readonly ExpressionTemplate LogMessageTemplate =
        new("[{@t:HH:mm:ss} {@l:u3}{#if SourceContext is not null} ({SourceContext}){#end}] {@m:lj}\n{@x}");

    private static readonly App App = new();

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [Obsolete("Obsolete")]
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
            .UseReactiveUI()
            .UseFAWindowing()
            .SetupWithLifetime(lifetime);

        #region Load configuration if exists

        var configBaseRepository = new ConfigBaseRepository();
        configBaseRepository.Load(DefaultConfigPath);

        var logLevel = configBaseRepository.ConfigBase?.logging?.logLevel ?? LogEventLevel.Verbose;
        string logOutputDirectory;
        if (configBaseRepository.ConfigBase != null)
        {
            if (configBaseRepository.ConfigBase.logging?.outputDirectory != null)
            {
                logOutputDirectory = configBaseRepository.ConfigBase.logging.outputDirectory;
            }
            else
            {
                logOutputDirectory = Path.Combine(configBaseRepository.ConfigBase.installedDirectory, "logs");
                if (!Directory.Exists(logOutputDirectory)) Directory.CreateDirectory(logOutputDirectory);
            }
        }
        else
        {
            logOutputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        #endregion

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithSensitiveDataMasking(options =>
            {
                options.MaskingOperators = new List<IMaskingOperator>
                {
                    new UsernameMaskingOperator(),
                    new DiscordBotTokenMaskingOperator()
                };
            })
            .WriteTo.Debug()
            .WriteTo.File(Path.Combine(logOutputDirectory, "AutoMuteUsPortable.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

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