using System.Runtime.Serialization;
using System.Text.Json;
using AutoMuteUsPortable.Shared.Entity.ConfigBaseNS;
using AutoMuteUsPortable.Shared.Utility;
using FluentValidation;

namespace AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;

public class ConfigBaseRepository : IConfigBaseRepository
{
    private string _filePath = "";
    public List<Entity.ConfigBaseNS.ConfigBase> ConfigBases { get; private set; } = new();

    public void LoadOrCreateDefault(string filePath)
    {
        _filePath = filePath;

        if (File.Exists(filePath))
        {
            var text = File.ReadAllText(filePath);
            var configBases = JsonSerializer.Deserialize<List<Entity.ConfigBaseNS.ConfigBase>>(text);
            if (configBases == null)
                throw new SerializationException("Failed to deserialize config file as List<ConfigBase>");

            var validator = new ListValidator<Entity.ConfigBaseNS.ConfigBase>(new ConfigBaseValidator());
            validator.ValidateAndThrow(configBases);

            ConfigBases = configBases;
        }
        else
        {
            var text = JsonSerializer.Serialize(ConfigBases, Utils.CustomJsonSerializerOptions);
            File.WriteAllText(filePath, text);
        }
    }

    public Entity.ConfigBaseNS.ConfigBase? FindUnique(string executableFilePath)
    {
        return ConfigBases.FirstOrDefault(x => x.executableFilePath == executableFilePath);
    }

    public void Update(string executableFilePath, Entity.ConfigBaseNS.ConfigBase config)
    {
        int idx;

        if ((idx = ConfigBases.FindIndex(x => x.executableFilePath == executableFilePath)) == -1)
            throw new KeyNotFoundException($"ConfigBase with executableFilePath {executableFilePath} is not found");

        var configBases = new List<Entity.ConfigBaseNS.ConfigBase>(ConfigBases);
        configBases[idx] = config;

        var validator = new ListValidator<Entity.ConfigBaseNS.ConfigBase>(new ConfigBaseValidator());
        validator.ValidateAndThrow(configBases);

        var text = JsonSerializer.Serialize(configBases, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        ConfigBases = configBases;
    }

    public void Delete(string executableFilePath)
    {
        int idx;

        if ((idx = ConfigBases.FindIndex(x => x.executableFilePath == executableFilePath)) == -1)
            throw new KeyNotFoundException($"ConfigBase with executableFilePath {executableFilePath} is not found");

        var configBases = new List<Entity.ConfigBaseNS.ConfigBase>(ConfigBases);
        configBases.RemoveAt(idx);

        var validator = new ListValidator<Entity.ConfigBaseNS.ConfigBase>(new ConfigBaseValidator());
        validator.ValidateAndThrow(configBases);

        var text = JsonSerializer.Serialize(configBases, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        ConfigBases = configBases;
    }
}