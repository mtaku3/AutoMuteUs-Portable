using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;

namespace AutoMuteUsPortable.Shared.Controller.Executor;

public class ExecutorControllerBase
{
    public ExecutorControllerBase(object executorConfiguration)
    {
        StandardOutput = new ReplaySubject<string>();
        StandardError = new ReplaySubject<string>();
    }

    public ExecutorControllerBase(object computedSimpleSettings,
        object executorConfigurationBase)
    {
        StandardOutput = new ReplaySubject<string>();
        StandardError = new ReplaySubject<string>();
    }

    public ExecutorConfiguration ExecutorConfiguration { get; protected set; }

    public bool IsRunning { get; protected set; }
    public event EventHandler? Started;
    public event EventHandler? Stopped;

    protected virtual void OnStart()
    {
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnStop()
    {
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public ISubject<string> StandardOutput { get; protected set; }
    public ISubject<string> StandardError { get; protected set; }

    public virtual Task Run(ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task Stop(ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task Restart(ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task Install(
        Dictionary<ExecutorType, ExecutorControllerBase> executors, ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task Update(
        Dictionary<ExecutorType, ExecutorControllerBase> executors, object oldExecutorConfiguration,
        ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }
}