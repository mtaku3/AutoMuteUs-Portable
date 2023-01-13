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

public partial class Executor
{
    public class Filters : ItemBaseFilters
    {
        /// <summary> Gets a Filter to Query data over the 'version' field in PocketBase </summary>
        public FieldFilterText Version => new("version");

        /// <summary> Gets a Filter to Query data over the 'hashes' field in PocketBase </summary>
        public FieldFilterText Hashes => new("hashes");

        /// <summary> Gets a Filter to Query data over the 'type' field in PocketBase </summary>
        public FieldFilterEnum<TypeEnum> Type => new("type");

        /// <summary> Gets a Filter to Query data over the 'download_url' field in PocketBase </summary>
        public FieldFilterText DownloadUrl => new("download_url");
    }
}