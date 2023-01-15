// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (https://automuteus-portable.pockethost.io)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using PocketBaseClient.Orm.Filters;

namespace AutoMuteUsPortable.PocketBaseClient.Models;

public partial class DownloadUrl
{
    public class Sorts : ItemBaseSorts
    {
        /// <summary>Makes a SortCommand to Order by the 'win_x86' field</summary>
        public SortCommand WinX86 => new("win_x86");

        /// <summary>Makes a SortCommand to Order by the 'win_x64' field</summary>
        public SortCommand WinX64 => new("win_x64");

        /// <summary>Makes a SortCommand to Order by the 'win_arm' field</summary>
        public SortCommand WinArm => new("win_arm");

        /// <summary>Makes a SortCommand to Order by the 'win_arm64' field</summary>
        public SortCommand WinArm64 => new("win_arm64");
    }
}