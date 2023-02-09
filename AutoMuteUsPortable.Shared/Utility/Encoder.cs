using System.Text;
using Standart.Hash.xxHash;

namespace AutoMuteUsPortable.Shared.Utility;

public static partial class Utils
{
    public static string EncodeExecutorDirectory(string type, string version)
    {
        var bytes = Encoding.UTF8.GetBytes($"{type} {version}");
        return xxHash3.ComputeHash(bytes, bytes.Length).ToString("x16");
    }
}