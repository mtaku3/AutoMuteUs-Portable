using System.Reactive;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.Pages;
using FluentAvalonia.UI.Media.Animation;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.ViewModels;

public class CreateDiscordBotPage3ViewModel : ViewModelBase
{
    public string Token
    {
        get => _token;
        set => this.RaiseAndSetIfChanged(ref _token, value);
    }

    private string _token = "";

    public bool IsValid
    {
        get => _isValid;
        set => this.RaiseAndSetIfChanged(ref _isValid, value);
    }

    private bool _isValid;

    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

    private void GoBack()
    {
        AppHost.Instance.Frame.GoBack();
    }

    public ReactiveCommand<Unit, Unit> GoForwardCommand { get; }

    private void GoForward()
    {
        if (AppHost.Instance.ConfigurationState is SimpleSettingsConfigurationState simpleSettingsConfigurationState)
            simpleSettingsConfigurationState.DiscordBotToken = Token;

        AppHost.Instance.Frame.Navigate(typeof(InstallPage), null, new SlideNavigationTransitionInfo());
    }

    public CreateDiscordBotPage3ViewModel()
    {
        GoBackCommand = ReactiveCommand.Create(GoBack);
        GoForwardCommand = ReactiveCommand.Create(GoForward, this.WhenAnyValue(vm => vm.IsValid));
    }
}