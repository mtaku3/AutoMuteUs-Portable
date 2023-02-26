using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutoMuteUsPortable.Core.Controller.ServerConfigurator;
using AutoMuteUsPortable.Core.Infrastructure.Config;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;
using AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;
using AutoMuteUsPortable.Shared.Utility;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using FluentAvalonia.UI.Media.Animation;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Pages;

public partial class InstallPage : ReactiveUserControl<InstallPageViewModel>
{
    public InstallPage()
    {
        InitializeComponent();
        ViewModel = new InstallPageViewModel
        {
            ProgressInfo = new ProgressInfo
            {
                name = "コンフィグを作成しています",
                IsIndeterminate = true
            }
        };

        this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, vm => vm.ProgressInfo, v => v.Progress.Value, x => x.progress * 100));

            Observable.Start(async () =>
            {
                var config = AppHost.Instance.ConfigurationState.CreateConfig();
                var serverConfigurator =
                    new ServerConfiguratorController(config, AppHost.Instance.PocketBaseClientApplication);

                var progress = new Subject<ProgressInfo>();
                progress.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => ViewModel.ProgressInfo = x);
                var taskProgress = new TaskProgress(progress, new List<string>
                {
                    "Configure executors",
                    "Install executors"
                });

                var configureProgress = taskProgress.GetSubjectProgress();
                await serverConfigurator.Configure(configureProgress);
                taskProgress.NextTask();

                var installProgress = taskProgress.GetSubjectProgress();
                await serverConfigurator.Install(installProgress);
                taskProgress.NextTask();

                if (!File.Exists(AppHost.DefaultConfigPath))
                {
                    var configBaseRepository = new ConfigBaseRepository();
                    configBaseRepository.LoadOrCreateDefault(AppHost.DefaultConfigPath);
                }

                var configRepository = new ConfigRepository(AppHost.ProcessPath);
                configRepository.Load(AppHost.DefaultConfigPath);
                configRepository.Create(config);

                Dispatcher.UIThread.Post(() =>
                {
                    AppHost.Instance.Frame.Navigate(typeof(InstallCompletedPage), null,
                        new SlideNavigationTransitionInfo());
                });
            }, RxApp.TaskpoolScheduler);
        });
    }
}