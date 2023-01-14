using AutoMuteUsPortable.Core.Entity.SimpleSettingsNS;

namespace AutoMuteUsPortable.Core.Entity.ComputedSimpleSettingsNS;

public class ComputedSimpleSettings : SimpleSettings
{
    public Port port { get; set; }
}

public class Port
{
    public int automuteus { get; set; }
    public int galactus { get; set; }
    public int broker { get; set; }
    public int postgresql { get; set; }
    public int redis { get; set; }
}