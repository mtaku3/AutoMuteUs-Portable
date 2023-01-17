using System.IO.Compression;
using System.Runtime.InteropServices;
using AutoMuteUsPortable.PocketBaseClient.Models;
using AutoMuteUsPortable.Shared.Utility.Dotnet.ZipFileProgressExtensionsNS;

namespace AutoMuteUsPortable.Shared.Utility;

public static partial class Utils
{
    public static void ExtractZip(string path, IProgress<double>? progress = null)
    {
        var directoryName = Path.GetDirectoryName(path);
        if (directoryName == null) throw new ArgumentException("Path is not a valid file path", nameof(path));

        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
        using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Read))
        {
            archive.ExtractToDirectory(directoryName, true, progress);
        }
    }

    public static async Task DownloadAsync(string url, string path, IProgress<double>? progress = null)
    {
        using (var client = new HttpClient())
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await client.DownloadDataAsync(url, fileStream, progress);
        }
    }

    public static string? GetDownloadUrl(DownloadUrl? downloadUrl)
    {
        var arch = RuntimeInformation.ProcessArchitecture;

        switch (arch)
        {
            case Architecture.Arm:
                return downloadUrl?.WinArm;
            case Architecture.Arm64:
                return downloadUrl?.WinArm64;
            case Architecture.X86:
                return downloadUrl?.WinX86;
            case Architecture.X64:
                return downloadUrl?.WinX64;
            default:
                throw new InvalidDataException($"{arch.ToString()} is not supported");
        }
    }

    public static string? GetChecksum(Checksum? checksum)
    {
        var arch = RuntimeInformation.ProcessArchitecture;

        switch (arch)
        {
            case Architecture.Arm:
                return checksum?.WinArm;
            case Architecture.Arm64:
                return checksum?.WinArm64;
            case Architecture.X86:
                return checksum?.WinX86;
            case Architecture.X64:
                return checksum?.WinX64;
            default:
                throw new InvalidDataException($"{arch.ToString()} is not supported");
        }
    }
}