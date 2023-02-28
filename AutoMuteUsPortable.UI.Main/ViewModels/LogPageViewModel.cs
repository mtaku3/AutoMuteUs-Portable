using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;
using AutoMuteUsPortable.UI.Main.Common;
using AutoMuteUsPortable.UI.Main.Views;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.ViewModels;

public class LogPageViewModel : ViewModelBase
{
    public ExecutorType Type
    {
        get => _type;
        set => this.RaiseAndSetIfChanged(ref _type, value);
    }

    private ExecutorType _type;

    public ExecutorState State
    {
        get => _state;
        set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    private ExecutorState _state = ExecutorState.Stopped;

    public ProgressInfo Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    private ProgressInfo _progress = new();

    public bool ProgressBarVisibility
    {
        get => _progressBarVisibility;
        set => this.RaiseAndSetIfChanged(ref _progressBarVisibility, value);
    }

    private bool _progressBarVisibility;

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    private string _text = string.Empty;

    public ReactiveCommand<Unit, Unit> RunCommand { get; }

    private void Run()
    {
        var progress = new Subject<ProgressInfo>();
        progress.Subscribe(x => Progress = x);
        ProgressBarVisibility = true;

        var cancellationTokenSource = new CancellationDisposable();
        MainWindow.Instance.CloseCTS.Add(cancellationTokenSource);

        Task.Run(async () =>
        {
            await AppHost.Instance.ServerConfiguratorController.executors[_type]
                .Run(progress, cancellationTokenSource.Token);
            MainWindow.Instance.CloseCTS.Remove(cancellationTokenSource, false);
        }).ContinueWith(_ => ProgressBarVisibility = false, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    private void Stop()
    {
        var progress = new Subject<ProgressInfo>();
        progress.Subscribe(x => Progress = x);
        ProgressBarVisibility = true;

        var cancellationTokenSource = new CancellationDisposable();
        MainWindow.Instance.CloseCTS.Add(cancellationTokenSource);

        Task.Run(async () =>
        {
            await AppHost.Instance.ServerConfiguratorController.executors[_type]
                .GracefullyStop(progress, cancellationTokenSource.Token);
            MainWindow.Instance.CloseCTS.Remove(cancellationTokenSource, false);
        }).ContinueWith(_ => ProgressBarVisibility = false, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public ReactiveCommand<Unit, Unit> RestartCommand { get; }

    private void Restart()
    {
        var progress = new Subject<ProgressInfo>();
        progress.Subscribe(x => Progress = x);
        ProgressBarVisibility = true;

        var cancellationTokenSource = new CancellationDisposable();
        MainWindow.Instance.CloseCTS.Add(cancellationTokenSource);

        Task.Run(async () =>
        {
            await AppHost.Instance.ServerConfiguratorController.executors[_type]
                .Restart(progress, cancellationTokenSource.Token);
            MainWindow.Instance.CloseCTS.Remove(cancellationTokenSource, false);
        }).ContinueWith(_ => ProgressBarVisibility = false, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public LogPageViewModel()
    {
        RunCommand = ReactiveCommand.Create(Run);
        StopCommand = ReactiveCommand.Create(Stop);
        RestartCommand = ReactiveCommand.Create(Restart);
    }

    public LogPageViewModel(ExecutorType type) : this()
    {
        Type = type;
    }
}

public enum ExecutorState
{
    Stopped,
    Running,
    Restarting
}