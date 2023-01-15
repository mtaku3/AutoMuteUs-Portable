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
    public class Filters : ItemBaseFilters
    {
        /// <summary> Gets a Filter to Query data over the 'win_86' field in PocketBase </summary>
        public FieldFilterText Win86 => new("win_86");

        /// <summary> Gets a Filter to Query data over the 'win_x64' field in PocketBase </summary>
        public FieldFilterText WinX64 => new("win_x64");

        /// <summary> Gets a Filter to Query data over the 'win_arm' field in PocketBase </summary>
        public FieldFilterText WinArm => new("win_arm");

        /// <summary> Gets a Filter to Query data over the 'win_arm64' field in PocketBase </summary>
        public FieldFilterText WinArm64 => new("win_arm64");
    }
}