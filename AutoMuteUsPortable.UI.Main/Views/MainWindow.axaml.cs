using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AutoMuteUsPortable.UI.Main.Common;
using AutoMuteUsPortable.UI.Main.Controls;
using AutoMuteUsPortable.UI.Main.Pages;
using AutoMuteUsPortable.UI.Main.ViewModels;
using Avalonia;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Controls;

namespace AutoMuteUsPortable.UI.Main.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();
    }

    public void InitializeFrames()
    {
        Frame.EntryStack.Add("GeneralPageFrame", new CustomPageStackEntry(null, null)
        {
            Instance = GeneralPageFrame.Instance
        });
        foreach (var (key, page) in AppHost.Instance.LogPageController.LogPages)
            Frame.EntryStack.Add($"LogPage.{key}", new CustomPageStackEntry(null, null)
            {
                Instance = page
            });

        Closing += HandleClosing;
    }

    public static MainWindow Instance { get; private set; }

    public static void Initialize()
    {
        if (Instance != null) throw new InvalidOperationException("MainWindow is already initialized");

        GeneralPageFrame.Initialize();
        Instance = new MainWindow();
        AppHost.Instance.LogPageController.Initialize();
        Instance.InitializeFrames();
        GeneralPageFrame.Instance.Frame.Navigate(typeof(StartingPage));
        Instance.NavigationView.SelectedItem = Instance.GeneralPageViewItem;
    }

    private void NavigationView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.IsSettingsSelected)
        {
        }
        else if (e.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag as string;
            if (tag == null) return;
            Frame.Navigate(tag);
        }
    }

    public WindowCloseMode WindowCloseMode = WindowCloseMode.CancelStartup;
    public CustomCompositeDisposable CloseCTS = new();

    private async void HandleClosing(object? sender, CancelEventArgs e)
    {
        if (WindowCloseMode == WindowCloseMode.CancelStartup)
        {
            e.Cancel = true;

            var confirmDialog = new TaskDialog
            {
                Title = "本当に強制終了しますか？",
                Content = "強制終了するとAutoMuteUsのデータが破損したり、次回正常に起動できなくなる可能性があります。\n"
                          + "起動終了後に安全に終了することをおすすめします。\n"
                          + "また、この操作は数分かかる場合があります。",
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
                Title = "強制終了しています",
                ShowProgressBar = true,
                XamlRoot = VisualRoot as Visual
            };
            taskDialog.Closing += (dialog, args) =>
            {
                // do nothing
            };

            taskDialog.ShowAsync();
            taskDialog.SetProgressBarState(0, TaskDialogProgressState.Indeterminate);

            Task.Run(() => CloseCTS.Clear()).ContinueWith(_ =>
            {
                taskDialog.Hide();
                ImmediatelyClose();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        else if (WindowCloseMode == WindowCloseMode.Graceful)
        {
            e.Cancel = true;

            Instance.NavigationView.SelectedItem = Instance.GeneralPageViewItem;
            GeneralPageFrame.Instance.Frame.Navigate(typeof(GracefullyClosingPage));
            var page = (GracefullyClosingPage)GeneralPageFrame.Instance.Frame.Content!;

            var cancellationTokenSource = new CancellationDisposable();
            CloseCTS.Add(cancellationTokenSource);
            page.Close(cancellationTokenSource.Token);

            WindowCloseMode = WindowCloseMode.Forcible;
        }
        else if (WindowCloseMode == WindowCloseMode.Forcible)
        {
            e.Cancel = true;

            var taskDialog = new TaskDialog
            {
                Title = "本当に強制終了しますか？",
                Content = "強制終了するとAutoMuteUsのデータが破損したり、次回正常に起動できなくなる可能性があります。",
                Buttons =
                {
                    TaskDialogButton.YesButton,
                    TaskDialogButton.NoButton
                },
                XamlRoot = VisualRoot as Visual
            };

            var result = (TaskDialogStandardResult)await taskDialog.ShowAsync();
            if (result != TaskDialogStandardResult.Yes) return;

            Instance.NavigationView.SelectedItem = Instance.GeneralPageViewItem;
            GeneralPageFrame.Instance.Frame.Navigate(typeof(ForciblyClosingPage));
            var page = (ForciblyClosingPage)GeneralPageFrame.Instance.Frame.Content!;

            CloseCTS.Clear();
            page.Close();
        }
    }

    public void ImmediatelyClose()
    {
        var tmp = WindowCloseMode;
        WindowCloseMode = WindowCloseMode.Immediate;
        Close();
        WindowCloseMode = tmp;
    }
}

public enum WindowCloseMode
{
    CancelStartup,
    Immediate,
    Graceful,
    Forcible
}