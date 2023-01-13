using AutoMuteUsPortable.Core.Entity.SimpleSettingsBaseNS;

namespace AutoMuteUsPortable.Core.Entity.SimpleSettingsNS;

public class SimpleSettings : SimpleSettingsBase
{
    public string discordToken { get; set; }
    public PostgresqlConfiguration postgresql { get; set; }
}

public class PostgresqlConfiguration
{
    public string username { get; set; }
    public string password { get; set; }
}