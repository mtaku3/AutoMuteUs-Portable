using System;
using System.Reactive;
using System.Text.RegularExpressions;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.Pages;
using FluentAvalonia.UI.Media.Animation;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.ViewModels;

public class PromptInstallationDirectoryPageViewModel : ViewModelBase
{
    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    private string _path = "";

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
            simpleSettingsConfigurationState.InstallationDirectory = Path;

        AppHost.Instance.Frame.Navigate(typeof(CreateDiscordBotPage1), null, new SlideNavigationTransitionInfo());
    }

    public PromptInstallationDirectoryPageViewModel()
    {
        this.WhenAnyValue(vm => vm.Path).Subscribe(value =>
        {
            IsValid = Regex.IsMatch(value, @"^[a-zA-Z]\:(/|\\)([A-Za-z0-9\-@_*]+(/|\\))*([A-Za-z0-9\-@_*]+)?$");
        });
        GoBackCommand = ReactiveCommand.Create(GoBack);
        GoForwardCommand = ReactiveCommand.Create(GoForward);
    }
}