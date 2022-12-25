namespace AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationBaseNS;

public class ExecutorConfigurationBase
{
    public string version { get; set; }
    public ExecutorType type { get; set; }
    public string binaryVersion { get; set; }
    public string binaryDirectory { get; set; }
}

public enum ExecutorType
{
    automuteus,
    galactus,
    postgresql,
    redis
}