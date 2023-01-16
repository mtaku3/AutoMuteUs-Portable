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

public partial class Redis
{
    public class Sorts : ItemBaseSorts
    {
        /// <summary>Makes a SortCommand to Order by the 'version' field</summary>
        public SortCommand Version => new("version");

        /// <summary>Makes a SortCommand to Order by the 'compatibleExecutors' field</summary>
        public SortCommand CompatibleExecutors => new("compatibleExecutors");

        /// <summary>Makes a SortCommand to Order by the 'download_url' field</summary>
        public SortCommand DownloadUrl => new("download_url");

        /// <summary>Makes a SortCommand to Order by the 'checksum' field</summary>
        public SortCommand Checksum => new("checksum");
    }
}