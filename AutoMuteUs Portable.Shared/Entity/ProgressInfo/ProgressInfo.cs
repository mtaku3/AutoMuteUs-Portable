namespace AutoMuteUs_Portable.Shared.Entity.ProgressInfo;

public class ProgressInfo : ProgressInfo<float>
{
}

public class ProgressInfo<T>
{
    public string name { get; set; }
    public T progress { get; set; }
}