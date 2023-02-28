using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;
using AutoMuteUsPortable.UI.Main.Common;
using AutoMuteUsPortable.UI.Main.ViewModels;
using AutoMuteUsPortable.UI.Main.Views;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.Pages;

public partial class ForciblyClosingPage : ReactiveUserControl<ClosingPageViewModel>
{
    public ForciblyClosingPage()
    {
        InitializeComponent();
        ViewModel = new ClosingPageViewModel();

        this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, vm => vm.ProgressInfo, v => v.Progress.Value, x => x.progress * 100));
        });
    }

    public void Close()
    {
        Task.Run(async () =>
        {
            var progress = new Subject<ProgressInfo>();
            progress.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => ViewModel!.ProgressInfo = x);
            await AppHost.Instance.ServerConfiguratorController.ForciblyStop(progress);

            await Dispatcher.UIThread.InvokeAsync(() => MainWindow.Instance.ImmediatelyClose());
        }, CancellationToken.None);
    }
}