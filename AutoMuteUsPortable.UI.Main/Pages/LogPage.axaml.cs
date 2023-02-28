using System;
using System.Reactive.Linq;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.UI.Main.ViewModels;
using AutoMuteUsPortable.UI.Main.Views;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.Pages;

public partial class LogPage : ReactiveUserControl<LogPageViewModel>
{
    public LogPage(ExecutorType executorType)
    {
        InitializeComponent();
        ViewModel = new LogPageViewModel(executorType);
        var mainWindowViewModel = MainWindow.Instance.ViewModel!;

        this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, vm => vm.State, v => v.StoppedState.IsVisible,
                x => x == ExecutorState.Stopped));
            d(this.OneWayBind(ViewModel, vm => vm.State, v => v.RunningState.IsVisible,
                x => x == ExecutorState.Running));
            d(this.OneWayBind(ViewModel, vm => vm.State, v => v.RestartingState.IsVisible,
                x => x == ExecutorState.Restarting));

            d(ViewModel.WhenAnyValue(vm => vm.State)
                .CombineLatest(mainWindowViewModel.WhenAnyValue(vm => vm.AppState),
                    (executorState, appState) => appState != AppState.Startup && executorState == ExecutorState.Stopped)
                .BindTo(this, v => v.RunButton.IsVisible));
            d(ViewModel.WhenAnyValue(vm => vm.State)
                .CombineLatest(mainWindowViewModel.WhenAnyValue(vm => vm.AppState),
                    (executorState, appState) => appState != AppState.Startup &&
                                                 executorState is ExecutorState.Running or ExecutorState.Restarting)
                .BindTo(this, v => v.StopButton.IsVisible));
            d(ViewModel.WhenAnyValue(vm => vm.State)
                .CombineLatest(mainWindowViewModel.WhenAnyValue(vm => vm.AppState),
                    (executorState, appState) => appState != AppState.Startup && executorState == ExecutorState.Running)
                .BindTo(this, v => v.RestartButton.IsVisible));

            d(this.OneWayBind(ViewModel, vm => vm.Progress, v => v.ProgressBar.Value, x => x.progress * 100));
        });
    }

    public LogPage()
    {
        throw new NotImplementedException();
    }
}