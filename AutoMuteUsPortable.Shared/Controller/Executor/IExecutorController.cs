using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;

namespace AutoMuteUsPortable.Shared.Controller.Executor;

public interface IExecutorController
{
    public static Dictionary<string, Parameter> InstallParameters;
    public static Dictionary<string, Parameter> UpdateParameters;
    public Task Run(ISubject<ProgressInfo>? progress = null);
    public Task Stop(ISubject<ProgressInfo>? progress = null);
    public Task Restart(ISubject<ProgressInfo>? progress = null);
    public Task Install(Dictionary<string, string> parameters, ISubject<ProgressInfo>? progress = null);
    public Task Update(Dictionary<string, string> parameters, ISubject<ProgressInfo>? progress = null);

    public Task InstallBySimpleSettings(dynamic simpleSettings, dynamic executorConfigurationBase,
        ISubject<ProgressInfo>? progress = null);

    public Task UpdateBySimpleSettings(dynamic simpleSettings, dynamic executorConfigurationBase,
        ISubject<ProgressInfo>? progress = null);
}