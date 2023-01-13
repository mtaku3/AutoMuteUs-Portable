using System.Text.Json.Serialization;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;

namespace AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;

public class ExecutorConfiguration : ExecutorConfigurationBase
{
    [JsonInclude] public override string binaryDirectory { get; set; }

    public Dictionary<string, string> environmentVariables { get; set; }
}