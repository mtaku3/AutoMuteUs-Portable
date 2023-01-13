using System.Security.Cryptography;

namespace AutoMuteUsPortable.Shared.Utility;

public static partial class Utils
{
    public static Dictionary<string, string> ParseHashesTxt(string txt)
    {
        var lines = txt.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var hashes = new Dictionary<string, string>();
        foreach (var line in lines)
            if (64 < line.Length)
            {
                var hash = line.Substring(0, 64);
                var fileName = line.Substring(65);
                hashes[fileName] = hash.ToLower();
            }

        return hashes;
    }

    public static List<string> CompareHashes(string root, Dictionary<string, string> hashes)
    {
        var ret = new List<string>();
        _CompareHashes(root, root, hashes, ref ret);
        return ret;
    }

    private static void _CompareHashes(string root, string target, Dictionary<string, string> hashes,
        ref List<string> ret)
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
                _CompareHashes(root, path, hashes, ref ret);
            }
            else
            {
                var fileInfo = new FileInfo(path);
                var relPath = Path.GetRelativePath(root, path).Replace(@"\", "/");

                if (!hashes.ContainsKey(relPath)) continue;

                var originalHash = hashes[relPath];

                using (var sha256 = SHA256.Create())
                using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var hash = BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                    if (originalHash != hash) ret.Add(relPath);
                }

                hashes.Remove(relPath);
            }

        if (root == target)
            ret.AddRange(hashes.Keys);
    }
}