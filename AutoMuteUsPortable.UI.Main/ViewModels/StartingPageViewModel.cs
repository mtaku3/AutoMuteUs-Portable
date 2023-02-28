using AutoMuteUsPortable.Shared.Entity.ProgressInfo;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.ViewModels;

public class StartingPageViewModel : ViewModelBase
{
    private ProgressInfo _progressInfo = new();

    public ProgressInfo ProgressInfo
    {
        get => _progressInfo;
        set => this.RaiseAndSetIfChanged(ref _progressInfo, value);
    }
}