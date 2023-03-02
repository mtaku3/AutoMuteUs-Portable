using System.Reactive;
using AutoMuteUsPortable.UI.Setup.Views;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.ViewModels;

public class InstallCompletedPageViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    private void Close()
    {
        MainWindow.Instance.Close();
    }

    public InstallCompletedPageViewModel()
    {
        CloseCommand = ReactiveCommand.Create(Close);
    }
}