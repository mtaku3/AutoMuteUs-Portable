using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Pages;

public partial class InstallCompletedPage : ReactiveUserControl<InstallCompletedPageViewModel>
{
    public InstallCompletedPage()
    {
        InitializeComponent();
        ViewModel = new InstallCompletedPageViewModel();
    }
}