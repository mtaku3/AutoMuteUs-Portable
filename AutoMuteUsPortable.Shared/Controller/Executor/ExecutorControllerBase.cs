using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;

namespace AutoMuteUsPortable.Shared.Controller.Executor;

public class ExecutorControllerBase
{
    public ExecutorControllerBase(object executorConfiguration)
    {
        OutputStream = new MemoryStream();
        ErrorStream = new MemoryStream();
    }

    public ExecutorControllerBase(object computedSimpleSettings,
        object executorConfigurationBase)
    {
        OutputStream = new MemoryStream();
        ErrorStream = new MemoryStream();
    }

    public ExecutorConfiguration ExecutorConfiguration { get; protected set; }

    public bool IsRunning { get; protected set; }
    public event EventHandler? Stopped;

    protected virtual void OnStop()
    {
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public Stream OutputStream { get; protected set; }
    public Stream ErrorStream { get; protected set; }

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