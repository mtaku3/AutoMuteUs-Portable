using System.Text.Json.Serialization;
using AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationBaseNS;

namespace AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationSSNS;

public class ExecutorConfigurationSS : ExecutorConfigurationBase
{
    public string _installedDirectory = "";
    [JsonIgnore] public override string binaryDirectory => Path.Combine(_installedDirectory, type.ToString());
    public Dictionary<string, string> environmentVariables { get; set; }
}