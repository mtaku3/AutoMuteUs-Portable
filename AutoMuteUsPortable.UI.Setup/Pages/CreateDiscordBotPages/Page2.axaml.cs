using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Pages;

public partial class CreateDiscordBotPage2 : ReactiveUserControl<CreateDiscordBotPage2ViewModel>
{
    public CreateDiscordBotPage2()
    {
        InitializeComponent();
        ViewModel = new CreateDiscordBotPage2ViewModel();
    }
}