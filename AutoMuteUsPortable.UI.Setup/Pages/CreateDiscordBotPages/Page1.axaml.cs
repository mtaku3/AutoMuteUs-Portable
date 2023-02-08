using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Pages;

public partial class CreateDiscordBotPage1 : ReactiveUserControl<CreateDiscordBotPage1ViewModel>
{
    public CreateDiscordBotPage1()
    {
        InitializeComponent();
        ViewModel = new CreateDiscordBotPage1ViewModel();
    }
}