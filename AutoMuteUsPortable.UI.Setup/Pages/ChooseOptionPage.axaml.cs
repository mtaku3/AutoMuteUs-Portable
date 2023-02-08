using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Pages;

public partial class ChooseOptionPage : ReactiveUserControl<ChooseOptionPageViewModel>
{
    public ChooseOptionPage()
    {
        InitializeComponent();
        ViewModel = new ChooseOptionPageViewModel();
    }
}