using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Controls;

public partial class FolderPicker : ReactiveUserControl<FolderPickerViewModel>
{
    public FolderPicker()
    {
        InitializeComponent();
        ViewModel = new FolderPickerViewModel();
    }
}