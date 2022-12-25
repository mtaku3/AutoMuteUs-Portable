namespace AutoMuteUs_Portable.Core.Infrastructure.Config;

public interface IConfigRepository
{
    public void Load(string filePath);
    public void Create(Entity.ConfigNS.Config config);
    public Entity.ConfigNS.Config? FindUnique(string executableFilePath);
    public void Update(string executableFilePath, Entity.ConfigNS.Config config);
    public void Delete(string executableFilePath);
}