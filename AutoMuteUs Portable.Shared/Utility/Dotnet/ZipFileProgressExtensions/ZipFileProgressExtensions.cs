//The MIT License (MIT)

//Copyright(c) 2022 mtaku3
//Copyright(c).NET Foundation and Contributors

//    All rights reserved.

//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.

//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.IO.Compression;
using AutoMuteUs_Portable.Shared.Utility.Dotnet.Common;

namespace AutoMuteUs_Portable.Shared.Utility.Dotnet.ZipFileProgressExtensionsNS;

public static class ZipFileProgressExtensions
{
    /// <summary>
    ///     Creates a file on the file system with the entry?s contents and the specified name. The last write time of the file
    ///     is set to the
    ///     entry?s last write time. This method does not allow overwriting of an existing file with the same name. Attempting
    ///     to extract explicit
    ///     directories (entries with names that end in directory separator characters) will not result in the creation of a
    ///     directory.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="ArgumentException">
    ///     destinationFileName is a zero-length string, contains only whitespace, or contains one or more
    ///     invalid characters as defined by InvalidPathChars. -or- destinationFileName specifies a directory.
    /// </exception>
    /// <exception cref="ArgumentNullException">destinationFileName is null.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
    ///     260 characters.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///     The path specified in destinationFileName is invalid (for example, it is on
    ///     an unmapped drive).
    /// </exception>
    /// <exception cref="IOException">
    ///     An I/O error has occurred. -or- The entry is currently open for writing.
    ///     -or- The entry has been deleted from the archive.
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///     destinationFileName is in an invalid format
    ///     -or- The ZipArchive that this entry belongs to was opened in a write-only mode.
    /// </exception>
    /// <exception cref="InvalidDataException">
    ///     The entry is missing from the archive or is corrupt and cannot be read
    ///     -or- The entry has been compressed using a compression method that is not supported.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The ZipArchive that this entry belongs to has been disposed.</exception>
    /// <param name="source">The zip archive entry to extract a file from.</param>
    /// <param name="destinationFileName">
    ///     The name of the file that will hold the contents of the entry.
    ///     The path is permitted to specify relative or absolute path information.
    ///     Relative path information is interpreted as relative to the current working directory.
    /// </param>
    public static void ExtractToFile(this ZipArchiveEntry source, string destinationFileName)
    {
        source.ExtractToFile(destinationFileName, false);
    }

    /// <summary>
    ///     Creates a file on the file system with the entry?s contents and the specified name.
    ///     The last write time of the file is set to the entry?s last write time.
    ///     This method does allows overwriting of an existing file with the same name.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="ArgumentException">
    ///     destinationFileName is a zero-length string, contains only whitespace,
    ///     or contains one or more invalid characters as defined by InvalidPathChars. -or- destinationFileName specifies a
    ///     directory.
    /// </exception>
    /// <exception cref="ArgumentNullException">destinationFileName is null.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
    ///     260 characters.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///     The path specified in destinationFileName is invalid
    ///     (for example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="IOException">
    ///     An I/O error has occurred.
    ///     -or- The entry is currently open for writing.
    ///     -or- The entry has been deleted from the archive.
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///     destinationFileName is in an invalid format
    ///     -or- The ZipArchive that this entry belongs to was opened in a write-only mode.
    /// </exception>
    /// <exception cref="InvalidDataException">
    ///     The entry is missing from the archive or is corrupt and cannot be read
    ///     -or- The entry has been compressed using a compression method that is not supported.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The ZipArchive that this entry belongs to has been disposed.</exception>
    /// <param name="source">The zip archive entry to extract a file from.</param>
    /// <param name="destinationFileName">
    ///     The name of the file that will hold the contents of the entry.
    ///     The path is permitted to specify relative or absolute path information.
    ///     Relative path information is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="overwrite">True to indicate overwrite.</param>
    public static void ExtractToFile(this ZipArchiveEntry source, string destinationFileName, bool overwrite)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (destinationFileName == null)
            throw new ArgumentNullException(nameof(destinationFileName));

        // Rely on FileStream's ctor for further checking destinationFileName parameter
        var fMode = overwrite ? FileMode.Create : FileMode.CreateNew;

        using (var fs = new FileStream(destinationFileName, fMode, FileAccess.Write, FileShare.None,
                   0x1000, false))
        {
            using (var es = source.Open())
            {
                es.CopyTo(fs);
            }

            ExtractExternalAttributes(fs, source);
        }

        try
        {
            File.SetLastWriteTime(destinationFileName, source.LastWriteTime.DateTime);
        }
        catch
        {
            // some OSes like Android (#35374) might not support setting the last write time, the extraction should not fail because of that
        }
    }

    private static void ExtractExternalAttributes(FileStream fs, ZipArchiveEntry entry)
    {
    }

    internal static void ExtractRelativeToDirectory(this ZipArchiveEntry source, string destinationDirectoryName)
    {
        source.ExtractRelativeToDirectory(destinationDirectoryName, false);
    }

    internal static void ExtractRelativeToDirectory(this ZipArchiveEntry source, string destinationDirectoryName,
        bool overwrite)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (destinationDirectoryName == null)
            throw new ArgumentNullException(nameof(destinationDirectoryName));

        // Note that this will give us a good DirectoryInfo even if destinationDirectoryName exists:
        var di = Directory.CreateDirectory(destinationDirectoryName);
        var destinationDirectoryFullPath = di.FullName;
        if (!destinationDirectoryFullPath.EndsWith(Path.DirectorySeparatorChar))
            destinationDirectoryFullPath += Path.DirectorySeparatorChar;

        var fileDestinationPath = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, source.FullName));

        if (!fileDestinationPath.StartsWith(destinationDirectoryFullPath, PathInternal.StringComparison))
            throw new IOException(SR.IO_ExtractingResultsInOutside);

        if (Path.GetFileName(fileDestinationPath).Length == 0)
        {
            // If it is a directory:

            if (source.Length != 0)
                throw new IOException(SR.IO_DirectoryNameWithData);

            Directory.CreateDirectory(fileDestinationPath);
        }
        else
        {
            // If it is a file:
            // Create containing directory:
            Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath)!);
            source.ExtractToFile(fileDestinationPath, overwrite);
        }
    }

    /// <summary>
    ///     <p>
    ///         Adds a file from the file system to the archive under the specified entry name.
    ///         The new entry in the archive will contain the contents of the file.
    ///         The last write time of the archive entry is set to the last write time of the file on the file system.
    ///         If an entry with the specified name already exists in the archive, a second entry will be created that has an
    ///         identical name.
    ///         If the specified source file has an invalid last modified time, the first datetime representable in the Zip
    ///         timestamp format
    ///         (midnight on January 1, 1980) will be used.
    ///     </p>
    ///     <p>
    ///         If an entry with the specified name already exists in the archive, a second entry will be created that has an
    ///         identical name.
    ///     </p>
    ///     <p>
    ///         Since no <code>CompressionLevel</code> is specified, the default provided by the implementation of the
    ///         underlying compression
    ///         algorithm will be used; the <code>ZipArchive</code> will not impose its own default.
    ///         (Currently, the underlying compression algorithm is provided by the
    ///         <code>System.IO.Compression.DeflateStream</code> class.)
    ///     </p>
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     sourceFileName is a zero-length string, contains only whitespace, or contains one or more
    ///     invalid characters as defined by InvalidPathChars. -or- entryName is a zero-length string.
    /// </exception>
    /// <exception cref="ArgumentNullException">sourceFileName or entryName is null.</exception>
    /// <exception cref="PathTooLongException">
    ///     In sourceFileName, the specified path, file name, or both exceed the system-defined maximum length.
    ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
    ///     260 characters.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///     The specified sourceFileName is invalid, (for example, it is on an
    ///     unmapped drive).
    /// </exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file specified by sourceFileName.</exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     sourceFileName specified a directory. -or- The caller does not have the
    ///     required permission.
    /// </exception>
    /// <exception cref="FileNotFoundException">The file specified in sourceFileName was not found. </exception>
    /// <exception cref="NotSupportedException">
    ///     sourceFileName is in an invalid format or the ZipArchive does not support
    ///     writing.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The ZipArchive has already been closed.</exception>
    /// <param name="destination">The zip archive to add the file to.</param>
    /// <param name="sourceFileName">
    ///     The path to the file on the file system to be copied from. The path is permitted to specify
    ///     relative or absolute path information. Relative path information is interpreted as relative to the current working
    ///     directory.
    /// </param>
    /// <param name="entryName">The name of the entry to be created.</param>
    /// <returns>A wrapper for the newly created entry.</returns>
    public static ZipArchiveEntry CreateEntryFromFile(this ZipArchive destination, string sourceFileName,
        string entryName)
    {
        return destination.DoCreateEntryFromFile(sourceFileName, entryName, null);
    }


    /// <summary>
    ///     <p>
    ///         Adds a file from the file system to the archive under the specified entry name.
    ///         The new entry in the archive will contain the contents of the file.
    ///         The last write time of the archive entry is set to the last write time of the file on the file system.
    ///         If an entry with the specified name already exists in the archive, a second entry will be created that has an
    ///         identical name.
    ///         If the specified source file has an invalid last modified time, the first datetime representable in the Zip
    ///         timestamp format
    ///         (midnight on January 1, 1980) will be used.
    ///     </p>
    ///     <p>
    ///         If an entry with the specified name already exists in the archive, a second entry will be created that has an
    ///         identical name.
    ///     </p>
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     sourceFileName is a zero-length string, contains only whitespace, or contains one or more
    ///     invalid characters as defined by InvalidPathChars. -or- entryName is a zero-length string.
    /// </exception>
    /// <exception cref="ArgumentNullException">sourceFileName or entryName is null.</exception>
    /// <exception cref="PathTooLongException">
    ///     In sourceFileName, the specified path, file name, or both exceed the system-defined maximum length.
    ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
    ///     260 characters.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///     The specified sourceFileName is invalid, (for example, it is on an
    ///     unmapped drive).
    /// </exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file specified by sourceFileName.</exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     sourceFileName specified a directory.
    ///     -or- The caller does not have the required permission.
    /// </exception>
    /// <exception cref="FileNotFoundException">The file specified in sourceFileName was not found. </exception>
    /// <exception cref="NotSupportedException">
    ///     sourceFileName is in an invalid format or the ZipArchive does not support
    ///     writing.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The ZipArchive has already been closed.</exception>
    /// <param name="destination">The zip archive to add the file to.</param>
    /// <param name="sourceFileName">
    ///     The path to the file on the file system to be copied from. The path is permitted to specify relative
    ///     or absolute path information. Relative path information is interpreted as relative to the current working
    ///     directory.
    /// </param>
    /// <param name="entryName">The name of the entry to be created.</param>
    /// <param name="compressionLevel">The level of the compression (speed/memory vs. compressed size trade-off).</param>
    /// <returns>A wrapper for the newly created entry.</returns>
    public static ZipArchiveEntry CreateEntryFromFile(this ZipArchive destination,
        string sourceFileName, string entryName, CompressionLevel compressionLevel)
    {
        return destination.DoCreateEntryFromFile(sourceFileName, entryName, compressionLevel);
    }

    internal static ZipArchiveEntry DoCreateEntryFromFile(this ZipArchive destination,
        string sourceFileName, string entryName, CompressionLevel? compressionLevel)
    {
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        if (sourceFileName == null)
            throw new ArgumentNullException(nameof(sourceFileName));

        if (entryName == null)
            throw new ArgumentNullException(nameof(entryName));

        // Checking of compressionLevel is passed down to DeflateStream and the IDeflater implementation
        // as it is a pluggable component that completely encapsulates the meaning of compressionLevel.

        // Argument checking gets passed down to FileStream's ctor and CreateEntry

        using (var fs = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read,
                   0x1000, false))
        {
            var entry = compressionLevel.HasValue
                ? destination.CreateEntry(entryName, compressionLevel.Value)
                : destination.CreateEntry(entryName);

            var lastWrite = File.GetLastWriteTime(sourceFileName);

            // If file to be archived has an invalid last modified time, use the first datetime representable in the Zip timestamp format
            // (midnight on January 1, 1980):
            if (lastWrite.Year < 1980 || lastWrite.Year > 2107)
                lastWrite = new DateTime(1980, 1, 1, 0, 0, 0);

            entry.LastWriteTime = lastWrite;

            SetExternalAttributes(fs, entry);

            using (var es = entry.Open())
            {
                fs.CopyTo(es);
            }

            return entry;
        }
    }

    private static void SetExternalAttributes(FileStream fs, ZipArchiveEntry entry)
    {
    }

    /// <summary>
    ///     Extracts all of the files in the archive to a directory on the file system. The specified directory may already
    ///     exist.
    ///     This method will create all subdirectories and the specified directory if necessary.
    ///     If there is an error while extracting the archive, the archive will remain partially extracted.
    ///     Each entry will be extracted such that the extracted file has the same relative path to destinationDirectoryName as
    ///     the
    ///     entry has to the root of the archive. If a file to be archived has an invalid last modified time, the first
    ///     datetime
    ///     representable in the Zip timestamp format (midnight on January 1, 1980) will be used.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     destinationDirectoryName is a zero-length string, contains only whitespace,
    ///     or contains one or more invalid characters as defined by InvalidPathChars.
    /// </exception>
    /// <exception cref="ArgumentNullException">destinationDirectoryName is null.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
    ///     260 characters.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="IOException">
    ///     An archive entry?s name is zero-length, contains only whitespace, or contains one or more invalid
    ///     characters as defined by InvalidPathChars. -or- Extracting an archive entry would have resulted in a destination
    ///     file that is outside destinationDirectoryName (for example, if the entry name contains parent directory accessors).
    ///     -or- An archive entry has the same name as an already extracted entry from the same archive.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="NotSupportedException">destinationDirectoryName is in an invalid format. </exception>
    /// <exception cref="InvalidDataException">
    ///     An archive entry was not found or was corrupt.
    ///     -or- An archive entry has been compressed using a compression method that is not supported.
    /// </exception>
    /// <param name="source">The zip archive to extract files from.</param>
    /// <param name="destinationDirectoryName">
    ///     The path to the directory on the file system.
    ///     The directory specified must not exist. The path is permitted to specify relative or absolute path information.
    ///     Relative path information is interpreted as relative to the current working directory.
    /// </param>
    public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName,
        IProgress<float>? progress = null)
    {
        source.ExtractToDirectory(destinationDirectoryName, false, progress);
    }

    /// <summary>
    ///     Extracts all of the files in the archive to a directory on the file system. The specified directory may already
    ///     exist.
    ///     This method will create all subdirectories and the specified directory if necessary.
    ///     If there is an error while extracting the archive, the archive will remain partially extracted.
    ///     Each entry will be extracted such that the extracted file has the same relative path to destinationDirectoryName as
    ///     the
    ///     entry has to the root of the archive. If a file to be archived has an invalid last modified time, the first
    ///     datetime
    ///     representable in the Zip timestamp format (midnight on January 1, 1980) will be used.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     destinationDirectoryName is a zero-length string, contains only whitespace,
    ///     or contains one or more invalid characters as defined by InvalidPathChars.
    /// </exception>
    /// <exception cref="ArgumentNullException">destinationDirectoryName is null.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
    ///     260 characters.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="IOException">
    ///     An archive entry?s name is zero-length, contains only whitespace, or contains one or more invalid
    ///     characters as defined by InvalidPathChars. -or- Extracting an archive entry would have resulted in a destination
    ///     file that is outside destinationDirectoryName (for example, if the entry name contains parent directory accessors).
    ///     -or- An archive entry has the same name as an already extracted entry from the same archive.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="NotSupportedException">destinationDirectoryName is in an invalid format. </exception>
    /// <exception cref="InvalidDataException">
    ///     An archive entry was not found or was corrupt.
    ///     -or- An archive entry has been compressed using a compression method that is not supported.
    /// </exception>
    /// <param name="source">The zip archive to extract files from.</param>
    /// <param name="destinationDirectoryName">
    ///     The path to the directory on the file system.
    ///     The directory specified must not exist. The path is permitted to specify relative or absolute path information.
    ///     Relative path information is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="overwriteFiles">True to indicate overwrite.</param>
    public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName,
        bool overwriteFiles, IProgress<float>? progress = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (destinationDirectoryName == null)
            throw new ArgumentNullException(nameof(destinationDirectoryName));

        var count = 0;
        foreach (var entry in source.Entries)
        {
            entry.ExtractRelativeToDirectory(destinationDirectoryName, overwriteFiles);
            progress?.Report((float)++count / source.Entries.Count * 100f);
        }
    }
}