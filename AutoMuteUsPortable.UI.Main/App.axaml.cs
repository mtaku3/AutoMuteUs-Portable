using AutoMuteUsPortable.UI.Main.Common;
using AutoMuteUsPortable.UI.Main.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AutoMuteUsPortable.UI.Main;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppHost.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow.Initialize();
            desktop.MainWindow = MainWindow.Instance;
        }

        base.OnFrameworkInitializationCompleted();
    }
}