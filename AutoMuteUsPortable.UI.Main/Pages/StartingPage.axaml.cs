using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;
using AutoMuteUsPortable.Shared.Utility;
using AutoMuteUsPortable.UI.Main.Common;
using AutoMuteUsPortable.UI.Main.ViewModels;
using AutoMuteUsPortable.UI.Main.Views;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.Pages;

public partial class StartingPage : ReactiveUserControl<StartingPageViewModel>
{
    public StartingPage()
    {
        InitializeComponent();
        ViewModel = new StartingPageViewModel();

        this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, vm => vm.ProgressInfo, v => v.Progress.Value, x => x.progress * 100));
        });

        Task.Run(async () =>
        {
            var progress = new Subject<ProgressInfo>();
            progress.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => ViewModel!.ProgressInfo = x);
            var taskProgress = new TaskProgress(progress, new List<string>
            {
                "Configure executors",
                "Run executors"
            });

            var cancellationTokenSource = new CancellationDisposable();
            MainWindow.Instance.CloseCTS.Add(cancellationTokenSource);

            var configureProgress = taskProgress.GetSubjectProgress();
            await AppHost.Instance.ServerConfiguratorController.Configure(configureProgress,
                cancellationTokenSource.Token);
            taskProgress.NextTask();

            foreach (var (type, executor) in AppHost.Instance.ServerConfiguratorController.executors)
            {
                AppHost.Instance.LogPageController.HookState(type, executor);
                AppHost.Instance.LogPageController.HookOutput(type, executor.StandardOutput);
                AppHost.Instance.LogPageController.HookOutput(type, executor.StandardError);
            }

            var runProgress = taskProgress.GetSubjectProgress();
            await AppHost.Instance.ServerConfiguratorController.Run(runProgress, cancellationTokenSource.Token);
            taskProgress.NextTask();

            MainWindow.Instance.CloseCTS.Remove(cancellationTokenSource, false);
            MainWindow.Instance.WindowCloseMode = WindowCloseMode.Graceful;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                MainWindow.Instance.ViewModel!.AppState = AppState.Running;
                GeneralPageFrame.Instance.Frame.Navigate(typeof(GeneralPage));
            });
        });
    }
}