using ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public AppState AppState
    {
        get => _appState;
        set => this.RaiseAndSetIfChanged(ref _appState, value);
    }

    private AppState _appState = AppState.Startup;
}

public enum AppState
{
    Startup,
    Running
}