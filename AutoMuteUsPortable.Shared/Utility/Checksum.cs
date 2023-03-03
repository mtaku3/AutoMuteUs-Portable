using Serilog;
using Standart.Hash.xxHash;

namespace AutoMuteUsPortable.Shared.Utility;

public static partial class Utils
{
    public static Dictionary<string, ulong> ParseChecksumText(string txt)
    {
        var lines = txt.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var res = new Dictionary<string, ulong>();
        foreach (var line in lines)
            if (18 < line.Length)
            {
                var checksum = line.Substring(0, 16);
                var fileName = line.Substring(17);
                res[fileName] = Convert.ToUInt64(checksum, 16);
            }

        Log.Verbose("Parsed checksums: {@Checksums}", res);

        return res;
    }

    public static List<string> CompareChecksum(string root, Dictionary<string, ulong> checksum,
        CancellationToken cancellationToken = default)
    {
        var ret = new List<string>();

        foreach (var item in checksum)
        {
            var filePath = Path.Combine(root, item.Key);
            if (!File.Exists(filePath))
            {
                ret.Add(item.Key);
                continue;
            }

            var bytes = File.ReadAllBytes(filePath);
            var computedChecksum = xxHash3.ComputeHash(bytes, bytes.Length);
            if (computedChecksum != item.Value) ret.Add(item.Key);

            cancellationToken.ThrowIfCancellationRequested();
        }

        Log.Debug("Found {Count} files with different checksums: {@InvalidFiles}", ret.Count, ret);

        return ret;
    }
}