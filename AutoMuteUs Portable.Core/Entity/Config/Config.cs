using AutoMuteUs_Portable.Core.Entity.SimpleSettingsNS;
using AutoMuteUs_Portable.Shared.Entity.ConfigBaseNS;
using AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationNS;

namespace AutoMuteUs_Portable.Core.Entity.ConfigNS;

public class Config : ConfigBase
{
    public ServerConfiguration serverConfiguration { get; set; }
}

public class ServerConfiguration
{
    public SimpleSettings? simpleSettings { get; set; }
    public List<ExecutorConfiguration>? advancedSettings { get; set; }
}