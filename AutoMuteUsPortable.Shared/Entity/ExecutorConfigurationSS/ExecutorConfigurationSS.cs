using System.Text.Json.Serialization;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using AutoMuteUsPortable.Shared.Utility;

namespace AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationSSNS;

public class ExecutorConfigurationSS : ExecutorConfigurationBase
{
    public string _installedDirectory = "";
    [JsonIgnore] public override string binaryDirectory => Path.Combine(_installedDirectory, type.ToString());

    [JsonIgnore]
    public override string executorDirectory =>
        Path.Combine(_installedDirectory, "Executors", Utils.EncodeExecutorDirectory(type.ToString(), version));

    public Dictionary<string, string> environmentVariables { get; set; }
}