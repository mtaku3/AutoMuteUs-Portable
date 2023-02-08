using AutoMuteUsPortable.Shared.Entity.ProgressInfo;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.Pages;
using FluentAvalonia.UI.Media.Animation;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.ViewModels;

public class InstallPageViewModel : ViewModelBase
{
    public ProgressInfo ProgressInfo
    {
        get => _progressInfo;
        set => this.RaiseAndSetIfChanged(ref _progressInfo, value);
    }

    private ProgressInfo _progressInfo = new();

    public void GoForward()
    {
        AppHost.Instance.Frame.Navigate(typeof(InstallCompletedPage), null, new SlideNavigationTransitionInfo());
    }
}