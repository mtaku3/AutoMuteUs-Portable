using System.Reactive;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.Pages;
using FluentAvalonia.UI.Media.Animation;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.ViewModels;

public class CreateDiscordBotPage1ViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

    private void GoBack()
    {
        AppHost.Instance.Frame.GoBack();
    }

    public ReactiveCommand<Unit, Unit> GoForwardCommand { get; }

    private void GoForward()
    {
        AppHost.Instance.Frame.Navigate(typeof(CreateDiscordBotPage2), null, new SlideNavigationTransitionInfo());
    }

    public CreateDiscordBotPage1ViewModel()
    {
        GoBackCommand = ReactiveCommand.Create(GoBack);
        GoForwardCommand = ReactiveCommand.Create(GoForward);
    }
}