using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Controller.Executor;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.UI.Main.Pages;
using AutoMuteUsPortable.UI.Main.ViewModels;
using Avalonia.Threading;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.Common;

public class LogPageController
{
    public readonly Dictionary<ExecutorType, LogPage> LogPages = new();

    private List<IDisposable> _disposables = new();

    private readonly IEnumerable<ExecutorConfigurationBase> _executorConfigurations;

    public LogPageController(IEnumerable<ExecutorConfigurationBase> executorConfigurations)
    {
        _executorConfigurations = executorConfigurations;
    }

    public void Initialize()
    {
        foreach (var executorConfiguration in _executorConfigurations)
            LogPages.Add(executorConfiguration.type, new LogPage(executorConfiguration.type));
    }

    public void HookOutput(ExecutorType type, ISubject<string> observable)
    {
        var logPage = LogPages.First(x => x.Key == type).Value;
        observable.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => logPage.ViewModel!.Text = x + "\n" + logPage.ViewModel!.Text);
    }

    public async void HookState(ExecutorType type, ExecutorControllerBase executorControllerBase)
    {
        var logPage = LogPages.First(x => x.Key == type).Value;
        await Dispatcher.UIThread.InvokeAsync(() =>
            logPage.ViewModel!.State =
                executorControllerBase.IsRunning ? ExecutorState.Running : ExecutorState.Stopped);
        executorControllerBase.Started += (sender, args) =>
            Dispatcher.UIThread.Post(() => logPage.ViewModel!.State = ExecutorState.Running);
        executorControllerBase.Stopped += (sender, args) =>
            Dispatcher.UIThread.Post(() => logPage.ViewModel!.State = ExecutorState.Stopped);
    }
}