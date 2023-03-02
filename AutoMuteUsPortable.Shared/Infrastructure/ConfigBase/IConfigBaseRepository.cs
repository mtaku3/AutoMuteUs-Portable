namespace AutoMuteUsPortable.Shared.Infrastructure.ConfigBase;

public interface IConfigBaseRepository
{
    public void Load(string filePath);
    public void Upsert(Entity.ConfigBaseNS.ConfigBase config);
    public void Delete();
}