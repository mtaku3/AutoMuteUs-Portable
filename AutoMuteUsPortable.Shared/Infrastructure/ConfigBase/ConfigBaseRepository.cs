using System.Runtime.Serialization;
using System.Text.Json;
using AutoMuteUsPortable.Shared.Entity.ConfigBaseNS;
using AutoMuteUsPortable.Shared.Utility;
using FluentValidation;
using Serilog;

namespace AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;

public class ConfigBaseRepository : IConfigBaseRepository
{
    private string _filePath = "";
    public Entity.ConfigBaseNS.ConfigBase? ConfigBase { get; private set; }

    public void Load(string filePath)
    {
        Log.Information("Loading config file from {FilePath}", filePath);

        _filePath = filePath;

        if (!File.Exists(filePath))
        {
            Log.Information("Config file does not exist at {FilePath}", filePath);
            return;
        }

        var text = File.ReadAllText(filePath);
        var configBase = JsonSerializer.Deserialize<Entity.ConfigBaseNS.ConfigBase>(text);
        if (configBase == null)
            throw new SerializationException("Failed to deserialize config file as List<ConfigBase>");

        var validator = new ConfigBaseValidator();
        validator.ValidateAndThrow(configBase);

        ConfigBase = configBase;
    }

    public void Upsert(Entity.ConfigBaseNS.ConfigBase config)
    {
        Log.Debug("Updating config to {@Config}", config);

        var validator = new ConfigBaseValidator();
        validator.ValidateAndThrow(config);

        var text = JsonSerializer.Serialize(config, Utils.CustomJsonSerializerOptions);
        File.WriteAllText(_filePath, text);

        ConfigBase = config;
    }

    public void Delete()
    {
        Log.Information("Deleting config file at {FilePath}", _filePath);

        File.Delete(_filePath);
        ConfigBase = null;
    }
}