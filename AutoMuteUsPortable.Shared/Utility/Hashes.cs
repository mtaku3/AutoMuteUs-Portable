using System.Security.Cryptography;

namespace AutoMuteUsPortable.Shared.Utility;

public static partial class Utils
{
    public static Dictionary<string, string> ParseHashesTxt(string txt)
    {
        var lines = txt.Split("\n");
        var hashes = new Dictionary<string, string>();
        foreach (var line in lines)
            if (40 < line.Length)
            {
                var hash = line.Substring(0, 40);
                var fileName = line.Substring(41);
                hashes[fileName] = hash.ToLower();
            }

        return hashes;
    }

    public static List<FileInfo> CompareHashes(string root, Dictionary<string, string> hashes)
    {
        var ret = new List<FileInfo>();
        _CompareHashes(root, root, hashes, ret);
        return ret;
    }

    private static void _CompareHashes(string root, string target, Dictionary<string, string> hashes,
        List<FileInfo> ret)
    {
        var files = Directory.GetFiles(target);
        var directories = Directory.GetDirectories(target);
        var values = files.Concat(directories).OrderBy(x =>
                File.GetAttributes(x).HasFlag(FileAttributes.Directory)
                    ? new DirectoryInfo(x).Name
                    : new FileInfo(x).Name
            )
            .ToList();
        foreach (var path in values)
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                _CompareHashes(root, path, hashes, ret);
            }
            else
            {
                var fileInfo = new FileInfo(path);
                var relPath = Path.GetRelativePath(root, path);

                if (!hashes.ContainsKey(relPath)) continue;

                var originalHash = hashes[relPath];

                using (var sha256 = SHA256.Create())
                {
                    using (var stream = fileInfo.Open(FileMode.Open))
                    {
                        var hash = sha256.ComputeHash(stream).ToString()?.ToLower();

                        if (originalHash != hash) ret.Append(fileInfo);
                    }
                }
            }
    }
}