namespace AutoMuteUs_Portable.Shared.Utility.ZipFileProgressExtensionsNS;

internal static class PathInternal
{
    /// <summary>Returns a comparison that can be used to compare file and directory names for equality.</summary>
    internal static StringComparison StringComparison =>
        IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

    /// <summary>Gets whether the system is case-sensitive.</summary>
    internal static bool IsCaseSensitive
    {
        get
        {
#if MS_IO_REDIST
                return false; // Windows is always case-insensitive
#else
            return !(OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsIOS() ||
                     OperatingSystem.IsTvOS() || OperatingSystem.IsWatchOS());
#endif
        }
    }
}