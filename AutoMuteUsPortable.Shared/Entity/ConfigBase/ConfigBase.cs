using Serilog.Events;

namespace AutoMuteUsPortable.Shared.Entity.ConfigBaseNS;

public class ConfigBase
{
    public string? name { get; set; }
    public Version? version { get; set; }
    public string installedDirectory { get; set; }
    public Logging? logging { get; set; }
}

public class Logging
{
    public string outputDirectory { get; set; }
    public LogEventLevel logLevel { get; set; }
}