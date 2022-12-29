using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using AutoMuteUs_Portable.Shared.Entity.ProgressInfo;
using AutoMuteUs_Portable.Shared.Infrastructure.ConfigBase;

namespace AutoMuteUs_Portable.Shared.Controller.AppUpdator
{
    public interface IAppUpdatorController
    {
        public Task<dynamic> Update(dynamic config, ISubject<ProgressInfo> progress);
    }
}
