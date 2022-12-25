namespace AutoMuteUs_Portable.Shared.Controller.Executor;

public interface IExecutorController
{
    public static Dictionary<string, Parameter> InstallParameters;
    public static Dictionary<string, Parameter> UpdateParameters;
    public void Run();
    public void Stop();
    public void Restart();
    public Task Install(Dictionary<string, string> parameters);
    public Task Update(Dictionary<string, string> parameters);
    public Task InstallBySimpleSettings(dynamic simpleSettings, dynamic executorConfigurationBase);
    public Task UpdateBySimpleSettings(dynamic simpleSettings, dynamic executorConfigurationBase);
}