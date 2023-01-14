using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;

namespace AutoMuteUsPortable.Shared.Controller.Executor;

public class ExecutorControllerBase
{
    public static Dictionary<string, Parameter> InstallParameters = new();
    public static Dictionary<string, Parameter> UpdateParameters = new();

    public ExecutorControllerBase(dynamic executorConfiguration)
    {
        throw new NotImplementedException();
    }

    public ExecutorControllerBase(dynamic computedSimpleSettings,
        dynamic executorConfigurationBase)
    {
        throw new NotImplementedException();
    }

    public bool IsRunning { get; protected set; }
    public event EventHandler Stopped;

    public virtual void OnStop()
    {
        Stopped?.Invoke(this, EventArgs.Empty);
    }

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

    public virtual Task Install(Dictionary<string, string> parameters, ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task Update(Dictionary<string, string> parameters, ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task InstallBySimpleSettings(dynamic simpleSettings, dynamic executorConfigurationBase,
        ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task UpdateBySimpleSettings(dynamic simpleSettings, dynamic executorConfigurationBase,
        ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }
}