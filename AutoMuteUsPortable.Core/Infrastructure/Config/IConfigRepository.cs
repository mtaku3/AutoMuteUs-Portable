namespace AutoMuteUsPortable.Core.Infrastructure.Config;

public interface IConfigRepository
{
    public void Load(string filePath);
    public void Upsert(Entity.ConfigNS.Config config);
    public void Delete();
}