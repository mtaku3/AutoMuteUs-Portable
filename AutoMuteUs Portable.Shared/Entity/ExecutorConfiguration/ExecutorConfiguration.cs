using AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationBaseNS;

namespace AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationNS;

public class ExecutorConfiguration : ExecutorConfigurationBase
{
    public Dictionary<string, string> environmentVariables { get; set; }
}