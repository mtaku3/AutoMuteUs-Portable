using AutoMuteUsPortable.UI.Main.Common;
using AutoMuteUsPortable.UI.Main.Views;
using Avalonia;
using Avalonia.Markup.Xaml;

namespace AutoMuteUsPortable;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void InitializeMainUI()
    {
        AppHost.Initialize();
        MainWindow.Initialize();
    }

    public void InitializeSetupUI()
    {
        UI.Setup.Common.AppHost.Initialize();
        UI.Setup.Views.MainWindow.Initialize();
    }
}