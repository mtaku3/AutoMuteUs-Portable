using System.Runtime.Serialization;
using System.Text.Json;
using AutoMuteUs_Portable.Core.Entity.ConfigNS;
using AutoMuteUs_Portable.Shared.Utility;
using FluentValidation;

namespace AutoMuteUs_Portable.Core.Infrastructure.Config;

public class ConfigRepository : IConfigRepository
{
    private List<Entity.ConfigNS.Config> _configs = new();
    private string _filePath = "";

    public void Load(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var configs = JsonSerializer.Deserialize<List<Entity.ConfigNS.Config>>(text, Utils.CustomJsonSerializerOptions);
        if (configs == null) throw new SerializationException("Failed to deserialize config file as List<Config>");

        foreach (var config in configs)
            if (config.serverConfiguration.simpleSettings != null)
                foreach (var executorConfiguration in config.serverConfiguration.simpleSettings.executorConfigurations)
                    executorConfiguration._installedDirectory = config.installedDirectory;

        var validator = new ListValidator<Entity.ConfigNS.Config>(new ConfigValidator());
        validator.ValidateAndThrow(configs);

        _filePath = filePath;
        _configs = configs;
    }

    public void Create(Entity.ConfigNS.Config config)
    {
        var configs = new List<Entity.ConfigNS.Config>(_configs) { config };

        var validator = new ListValidator<Entity.ConfigNS.Config>(new ConfigValidator());
        validator.ValidateAndThrow(configs);

        var text = JsonSerializer.Serialize(configs, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        _configs = configs;
    }

    public Entity.ConfigNS.Config? FindUnique(string executableFilePath)
    {
        return _configs.FirstOrDefault(x => x.executableFilePath == executableFilePath);
    }

    public void Update(string executableFilePath, Entity.ConfigNS.Config config)
    {
        int idx;

        if ((idx = _configs.FindIndex(x => x.executableFilePath == executableFilePath)) == -1)
            throw new KeyNotFoundException($"Config with executableFilePath {executableFilePath} is not found");

        var configs = new List<Entity.ConfigNS.Config>(_configs);
        configs[idx] = config;

        var validator = new ListValidator<Entity.ConfigNS.Config>(new ConfigValidator());
        validator.ValidateAndThrow(configs);

        var text = JsonSerializer.Serialize(configs, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        _configs = configs;
    }

    public void Delete(string executableFilePath)
    {
        int idx;

        if ((idx = _configs.FindIndex(x => x.executableFilePath == executableFilePath)) == -1)
            throw new KeyNotFoundException($"Config with executableFilePath {executableFilePath} is not found");

        var configs = new List<Entity.ConfigNS.Config>(_configs);
        configs.RemoveAt(idx);

        var validator = new ListValidator<Entity.ConfigNS.Config>(new ConfigValidator());
        validator.ValidateAndThrow(configs);

        var text = JsonSerializer.Serialize(configs, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        _configs = configs;
    }
}