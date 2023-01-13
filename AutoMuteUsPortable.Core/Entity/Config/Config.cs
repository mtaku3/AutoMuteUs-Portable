using AutoMuteUsPortable.Core.Entity.SimpleSettingsNS;
using AutoMuteUsPortable.Shared.Entity.ConfigBaseNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;

namespace AutoMuteUsPortable.Core.Entity.ConfigNS;

public class Config : ConfigBase
{
    public ServerConfiguration serverConfiguration { get; set; }
}

public class ServerConfiguration
{
    public SimpleSettings? simpleSettings { get; set; }
    public List<ExecutorConfiguration>? advancedSettings { get; set; }
}