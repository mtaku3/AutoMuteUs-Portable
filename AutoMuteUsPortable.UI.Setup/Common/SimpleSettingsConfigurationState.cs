using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMuteUsPortable.Core.Entity.ConfigNS;
using AutoMuteUsPortable.Core.Entity.SimpleSettingsNS;
using AutoMuteUsPortable.PocketBaseClient;
using AutoMuteUsPortable.PocketBaseClient.Models;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationSSNS;
using AutoMuteUsPortable.UI.Setup.Utils;

namespace AutoMuteUsPortable.UI.Setup.Common;

public class SimpleSettingsConfigurationState : IConfigurationState
{
    private readonly PocketBaseClientApplication _pbClient;

    public SimpleSettingsConfigurationState(PocketBaseClientApplication pbClient)
    {
        _pbClient = pbClient;
    }

    public string DiscordBotToken = "";
    public string InstallationDirectory = "";

    public Config CreateConfig()
    {
        var version = Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
        var application = _pbClient.Data.ApplicationCollection.First(x => x.Version == version);
        var compatibleExecutors = application.CompatibleExecutors;

        var redisExecutor = compatibleExecutors.Where(x => x.Type == Executor.TypeEnum.Redis)
            .OrderByDescending(x => x.Version).First();
        var postgresqlExecutor = compatibleExecutors.Where(x => x.Type == Executor.TypeEnum.Postgresql)
            .OrderByDescending(x => x.Version).First();
        var galactusExecutor = compatibleExecutors.Where(x => x.Type == Executor.TypeEnum.Galactus)
            .OrderByDescending(x => x.Version).First();
        var automuteusExecutor = compatibleExecutors.Where(x => x.Type == Executor.TypeEnum.Automuteus)
            .OrderByDescending(x => x.Version).First();

        var redis = _pbClient.Data.RedisCollection.OrderByDescending(x => x.Version)
            .First(x => x.CompatibleExecutors.Contains(redisExecutor));
        var postgresql = _pbClient.Data.PostgresqlCollection.OrderByDescending(x => x.Version)
            .First(x => x.CompatibleExecutors.Contains(redisExecutor));
        var galactus = _pbClient.Data.GalactusCollection.OrderByDescending(x => x.Version)
            .First(x => x.CompatibleExecutors.Contains(redisExecutor));
        var automuteus = _pbClient.Data.AutomuteusCollection.OrderByDescending(x => x.Version)
            .First(x => x.CompatibleExecutors.Contains(redisExecutor));

        var config = new Config
        {
            version = version,
            installedDirectory = InstallationDirectory,
            serverConfiguration = new ServerConfiguration
            {
                simpleSettings = new SimpleSettings
                {
                    discordToken = DiscordBotToken,
                    postgresql = new PostgresqlConfiguration
                    {
                        username = Random.Shared.RandomString(25),
                        password = Random.Shared.RandomString(25)
                    },
                    executorConfigurations = new List<ExecutorConfigurationSS>
                    {
                        new()
                        {
                            _installedDirectory = InstallationDirectory,
                            type = ExecutorType.redis,
                            version = redisExecutor.Version!,
                            binaryVersion = redis.Version!
                        },
                        new()
                        {
                            _installedDirectory = InstallationDirectory,
                            type = ExecutorType.postgresql,
                            version = postgresqlExecutor.Version!,
                            binaryVersion = postgresql.Version!
                        },
                        new()
                        {
                            _installedDirectory = InstallationDirectory,
                            type = ExecutorType.galactus,
                            version = galactusExecutor.Version!,
                            binaryVersion = galactus.Version!
                        },
                        new()
                        {
                            _installedDirectory = InstallationDirectory,
                            type = ExecutorType.automuteus,
                            version = automuteusExecutor.Version!,
                            binaryVersion = automuteus.Version!
                        }
                    }
                }
            }
        };

        return config;
    }
}