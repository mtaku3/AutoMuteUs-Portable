using System.Runtime.Serialization;
using System.Text.Json;
using AutoMuteUsPortable.Core.Entity.ConfigNS;
using AutoMuteUsPortable.Shared.Utility;
using FluentValidation;

namespace AutoMuteUsPortable.Core.Infrastructure.Config;

public class ConfigRepository : IConfigRepository
{
    private string _filePath = "";

    public Entity.ConfigNS.Config? Config { get; private set; }

    public void Load(string filePath)
    {
        _filePath = filePath;

        if (!File.Exists(filePath)) return;

        var text = File.ReadAllText(filePath);
        var config = JsonSerializer.Deserialize<Entity.ConfigNS.Config>(text, Utils.CustomJsonSerializerOptions);
        if (config == null) throw new SerializationException("Failed to deserialize config file as List<Config>");

        if (config.serverConfiguration.IsSimpleSettingsUsed)
            foreach (var executorConfiguration in config.serverConfiguration.simpleSettings!.executorConfigurations)
                executorConfiguration._installedDirectory = config.installedDirectory;

        var validator = new ConfigValidator();
        validator.ValidateAndThrow(config);

        _filePath = filePath;
        Config = config;
    }

    public void Upsert(Entity.ConfigNS.Config config)
    {
        var validator = new ConfigValidator();
        validator.ValidateAndThrow(config);

        var text = JsonSerializer.Serialize(config, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        Config = config;
    }

    public void Delete()
    {
        File.Delete(_filePath);
        Config = null;
    }
}