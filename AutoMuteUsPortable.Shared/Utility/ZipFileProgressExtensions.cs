using System.IO.Compression;

namespace AutoMuteUsPortable.Shared.Utility;

public static class ZipFileProgressExtensions
{
    public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ExtractToDirectory(source, destinationDirectoryName, false, progress, cancellationToken);
    }

    public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, bool overwriteFiles,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (destinationDirectoryName == null)
            throw new ArgumentNullException(nameof(destinationDirectoryName));

        var totalCount = source.Entries.Count;
        var extractedCount = 0;

        foreach (var entry in source.Entries)
        {
            var entryDestinationPath = Path.Combine(destinationDirectoryName, entry.FullName);
            if (entry.Length == 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(entryDestinationPath) ??
                                          throw new InvalidOperationException());
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(entryDestinationPath) ??
                                          throw new InvalidOperationException());
                using (var entryStream = entry.Open())
                using (var destinationStream = File.Create(entryDestinationPath))
                {
                    var buffer = new byte[81920];
                    int bytesRead;
                    while ((bytesRead = entryStream.Read(buffer, 0, buffer.Length)) > 0)
                        destinationStream.Write(buffer, 0, bytesRead);
                }
            }

            extractedCount++;
            progress?.Report((double)extractedCount / totalCount);

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}