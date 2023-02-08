using System.Reactive;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.Pages;
using FluentAvalonia.UI.Media.Animation;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.ViewModels;

public class ChooseOptionPageViewModel : ViewModelBase
{
    public bool IsSimpleSettingsChecked
    {
        get => _isSimpleSettingsChecked;
        set => this.RaiseAndSetIfChanged(ref _isSimpleSettingsChecked, value);
    }

    private bool _isSimpleSettingsChecked = true;

    public ReactiveCommand<Unit, Unit> GoForwardCommand { get; }

    private void GoForward()
    {
        if (0 < AppHost.Instance.Frame.ForwardStack.Count) AppHost.Instance.Frame.ForwardStack.Clear();

        if (IsSimpleSettingsChecked)
            AppHost.Instance.ConfigurationState =
                new SimpleSettingsConfigurationState(AppHost.Instance.PocketBaseClientApplication);

        AppHost.Instance.Frame.Navigate(typeof(PromptInstallationDirectoryPage), null,
            new SlideNavigationTransitionInfo());
    }

    public ChooseOptionPageViewModel()
    {
        GoForwardCommand = ReactiveCommand.Create(GoForward);
    }
}