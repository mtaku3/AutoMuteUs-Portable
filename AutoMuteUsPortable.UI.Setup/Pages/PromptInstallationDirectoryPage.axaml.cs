using System;
using System.IO;
using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Pages;

public partial class PromptInstallationDirectoryPage : ReactiveUserControl<PromptInstallationDirectoryPageViewModel>
{
    public PromptInstallationDirectoryPage()
    {
        InitializeComponent();
        ViewModel = new PromptInstallationDirectoryPageViewModel();
        FolderPicker.ViewModel!.Path =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AutoMuteUsPortable");

        this.WhenActivated(d =>
        {
            d(FolderPicker.ViewModel!.WhenAnyValue(vm => vm.Path).Subscribe(value => ViewModel.Path = value));
        });
    }
}