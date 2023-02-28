using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.Pages;
using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Controls;

namespace AutoMuteUsPortable.UI.Setup.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();

        Closing += HandleClosing;
    }

    public static MainWindow Instance { get; private set; }

    public static void Initialize()
    {
        if (Instance != null) throw new InvalidOperationException("Main window is already initialized");

        Instance = new MainWindow();
        AppHost.Instance.Frame.Navigate(typeof(ChooseOptionPage));
    }

    public CancellationTokenSource? CloseCTS;

    private async void HandleClosing(object? sender, CancelEventArgs e)
    {
        if (CloseCTS != null)
        {
            e.Cancel = true;

            var confirmDialog = new TaskDialog
            {
                Content = "本当にキャンセルしますか？",
                Buttons =
                {
                    TaskDialogButton.YesButton,
                    TaskDialogButton.NoButton
                },
                XamlRoot = VisualRoot as Visual
            };

            var result = (TaskDialogStandardResult)await confirmDialog.ShowAsync();
            if (result != TaskDialogStandardResult.Yes) return;

            var taskDialog = new TaskDialog
            {
                Content = "キャンセルしています",
                ShowProgressBar = true,
                XamlRoot = VisualRoot as Visual
            };
            taskDialog.Closing += (dialog, args) =>
            {
                // do nothing
            };

            taskDialog.ShowAsync();
            taskDialog.SetProgressBarState(0, TaskDialogProgressState.Indeterminate);

            Task.Run(() => CloseCTS?.Cancel()).ContinueWith(_ =>
                {
                    taskDialog.Hide();
                    CloseCTS = null;
                    Close();
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}