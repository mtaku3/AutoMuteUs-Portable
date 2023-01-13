using System;
using AutoMuteUsPortable.Core.Infrastructure.Config;
using AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;
using AutoMuteUsPortable.UI.Main;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMuteUsPortable;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();

        #region Dependency Injection - Repository

        serviceCollection.AddSingleton<IConfigBaseRepository, ConfigBaseRepository>();
        serviceCollection.AddSingleton<IConfigRepository, ConfigRepository>();

        #endregion

        var service = serviceCollection.BuildServiceProvider();

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