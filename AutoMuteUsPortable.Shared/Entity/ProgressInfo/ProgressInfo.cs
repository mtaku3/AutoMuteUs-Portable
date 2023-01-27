namespace AutoMuteUsPortable.Shared.Entity.ProgressInfo;

public class ProgressInfo : ProgressInfo<double>
{
}

public class ProgressInfo<T>
{
    public string name { get; set; }
    public T progress { get; set; } = default!;
    public bool IsIndeterminate { get; set; } = false;
}