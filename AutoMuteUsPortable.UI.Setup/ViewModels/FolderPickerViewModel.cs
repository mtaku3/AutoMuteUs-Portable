using System.IO;
using System.Threading.Tasks;
using AutoMuteUsPortable.UI.Setup.Views;
using Avalonia.Controls;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.ViewModels;

public class FolderPickerViewModel : ViewModelBase
{
    private string _path = "";
    public string Title { get; set; } = "";

    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public async Task OpenDialog()
    {
        // TODO: Use StorageProvider instead of OpenFolderDialog.
        // But for now, 11.0.0-preview4 does not contain StorageProviderExtensions so that path cannot be retrieved from result.
        var dialog = new OpenFolderDialog
        {
            Title = Title,
            Directory = Directory.Exists(Path) ? Path : null
        };
        var result = await dialog.ShowAsync(MainWindow.Instance);
        if (result != null) Path = result;
    }
}