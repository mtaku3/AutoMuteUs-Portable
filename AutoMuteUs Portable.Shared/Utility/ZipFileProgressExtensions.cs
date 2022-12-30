using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMuteUs_Portable.Shared.Utility
{
    public static class ZipFileProgressExtensions
    {
        public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, IProgress<float>? progress = null, bool overwriteFiles = false)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destinationDirectoryName);

            int count = 0;
            foreach (ZipArchiveEntry entry in source.Entries)
            {
                entry.ExtractRelativeToDirectory(destinationDirectoryName, overwriteFiles);
                progress?.Report((float)++count / source.Entries.Count * 100f);
            }
        }
    }
}
