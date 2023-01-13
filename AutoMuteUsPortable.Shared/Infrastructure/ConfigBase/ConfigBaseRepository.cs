using System.Runtime.Serialization;
using System.Text.Json;
using AutoMuteUsPortable.Shared.Entity.ConfigBaseNS;
using AutoMuteUsPortable.Shared.Utility;
using FluentValidation;

namespace AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;

public class ConfigBaseRepository : IConfigBaseRepository
{
    private List<Entity.ConfigBaseNS.ConfigBase> _configBases = new();
    private string _filePath = "";

    public void LoadOrCreateDefault(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var configBases = JsonSerializer.Deserialize<List<Entity.ConfigBaseNS.ConfigBase>>(text);
        if (configBases == null)
            throw new SerializationException("Failed to deserialize config file as List<ConfigBase>");

        var validator = new ListValidator<Entity.ConfigBaseNS.ConfigBase>(new ConfigBaseValidator());
        validator.ValidateAndThrow(configBases);

        _filePath = filePath;
        _configBases = configBases;
    }

    public Entity.ConfigBaseNS.ConfigBase? FindUnique(string executableFilePath)
    {
        return _configBases.FirstOrDefault(x => x.executableFilePath == executableFilePath);
    }

    public void Update(string executableFilePath, Entity.ConfigBaseNS.ConfigBase config)
    {
        int idx;

        if ((idx = _configBases.FindIndex(x => x.executableFilePath == executableFilePath)) == -1)
            throw new KeyNotFoundException($"ConfigBase with executableFilePath {executableFilePath} is not found");

        var configBases = new List<Entity.ConfigBaseNS.ConfigBase>(_configBases);
        configBases[idx] = config;

        var validator = new ListValidator<Entity.ConfigBaseNS.ConfigBase>(new ConfigBaseValidator());
        validator.ValidateAndThrow(configBases);

        var text = JsonSerializer.Serialize(configBases, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        _configBases = configBases;
    }

    public void Delete(string executableFilePath)
    {
        int idx;

        if ((idx = _configBases.FindIndex(x => x.executableFilePath == executableFilePath)) == -1)
            throw new KeyNotFoundException($"ConfigBase with executableFilePath {executableFilePath} is not found");

        var configBases = new List<Entity.ConfigBaseNS.ConfigBase>(_configBases);
        configBases.RemoveAt(idx);

        var validator = new ListValidator<Entity.ConfigBaseNS.ConfigBase>(new ConfigBaseValidator());
        validator.ValidateAndThrow(configBases);

        var text = JsonSerializer.Serialize(configBases, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        _configBases = configBases;
    }
}