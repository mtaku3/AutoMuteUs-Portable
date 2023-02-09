namespace AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;

public class ExecutorConfigurationBase
{
    public string version { get; set; }
    public ExecutorType type { get; set; }
    public string binaryVersion { get; set; }
    public virtual string binaryDirectory { get; set; }
    public virtual string executorDirectory { get; set; }
}

public enum ExecutorType
{
    automuteus,
    galactus,
    postgresql,
    redis
}