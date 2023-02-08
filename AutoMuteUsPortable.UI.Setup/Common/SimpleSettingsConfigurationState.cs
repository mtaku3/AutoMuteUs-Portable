using System;
using System.Collections.Generic;
using System.Linq;
using AutoMuteUsPortable.Core.Entity.ConfigNS;
using AutoMuteUsPortable.Core.Entity.SimpleSettingsNS;
using AutoMuteUsPortable.PocketBaseClient;
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
        var redis = _pbClient.Data.RedisCollection.First();
        var postgresql = _pbClient.Data.PostgresqlCollection.First();
        var galactus = _pbClient.Data.GalactusCollection.First();
        var automuteus = _pbClient.Data.AutomuteusCollection.First();

        var redisExecutor = redis.CompatibleExecutors.OrderByDescending(x => x.Created).First();
        var postgresqlExecutor = postgresql.CompatibleExecutors.OrderByDescending(x => x.Created).First();
        var galactusExecutor = galactus.CompatibleExecutors.OrderByDescending(x => x.Created).First();
        var automuteusExecutor = automuteus.CompatibleExecutors.OrderByDescending(x => x.Created).First();

        var config = new Config
        {
            executableFilePath = Environment.ProcessPath!,
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