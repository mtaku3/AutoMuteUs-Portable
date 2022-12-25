namespace AutoMuteUs_Portable.Shared.Infrastructure.ConfigBase;

public interface IConfigBaseRepository
{
    public void LoadOrCreateDefault(string filePath);
    public Entity.ConfigBaseNS.ConfigBase? FindUnique(string executableFilePath);
    public void Update(string executableFilePath, Entity.ConfigBaseNS.ConfigBase config);
    public void Delete(string executableFilePath);
}