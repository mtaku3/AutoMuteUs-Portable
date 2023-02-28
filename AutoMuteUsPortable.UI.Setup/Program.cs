using System;
using Avalonia;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Windowing;
#if DEBUG
using DotNetEnv;
#endif

namespace AutoMuteUsPortable.UI.Setup;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        Env.TraversePath().Load();
#endif

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI()
            .UseFAWindowing();
    }
}