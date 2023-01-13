using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;

namespace AutoMuteUsPortable.Shared.Controller.AppUpdator;

public interface IAppUpdatorController
{
    public Task<dynamic> Update(dynamic config, ISubject<ProgressInfo> progress);
}