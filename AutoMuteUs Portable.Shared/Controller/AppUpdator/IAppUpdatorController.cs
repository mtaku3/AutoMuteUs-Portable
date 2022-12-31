using System.Reactive.Subjects;
using AutoMuteUs_Portable.Shared.Entity.ProgressInfo;

namespace AutoMuteUs_Portable.Shared.Controller.AppUpdator;

public interface IAppUpdatorController
{
    public Task<dynamic> Update(dynamic config, ISubject<ProgressInfo> progress);
}