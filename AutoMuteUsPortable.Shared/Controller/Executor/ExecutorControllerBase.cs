﻿using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;

namespace AutoMuteUsPortable.Shared.Controller.Executor;

public class ExecutorControllerBase
{
    public static Dictionary<string, Parameter> InstallParameters;
    public static Dictionary<string, Parameter> UpdateParameters;
    public readonly ExecutorConfiguration ExecutorConfiguration;

    public ExecutorControllerBase(object executorConfiguration)
    {
    }

    public ExecutorControllerBase(object computedSimpleSettings,
        object executorConfigurationBase)
    {
    }

    public bool IsRunning { get; protected set; }
    public event EventHandler? Stopped;

    protected virtual void OnStop()
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

    public virtual Task Install(Dictionary<string, string> parameters,
        Dictionary<ExecutorType, ExecutorControllerBase> executors, ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task Update(Dictionary<string, string> parameters,
        Dictionary<ExecutorType, ExecutorControllerBase> executors, ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task InstallBySimpleSettings(object simpleSettings, object executorConfigurationBase,
        Dictionary<ExecutorType, ExecutorControllerBase> executors,
        ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task UpdateBySimpleSettings(object simpleSettings, object executorConfigurationBase,
        Dictionary<ExecutorType, ExecutorControllerBase> executors,
        ISubject<ProgressInfo>? progress = null)
    {
        throw new NotImplementedException();
    }
}