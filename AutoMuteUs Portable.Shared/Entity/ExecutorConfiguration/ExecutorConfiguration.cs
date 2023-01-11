using System.Text.Json.Serialization;
using AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationBaseNS;

namespace AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationNS;

public class ExecutorConfiguration : ExecutorConfigurationBase
{
    [JsonInclude] public override string binaryDirectory { get; set; }

    public Dictionary<string, string> environmentVariables { get; set; }
}