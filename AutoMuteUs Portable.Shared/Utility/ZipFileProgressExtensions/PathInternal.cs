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