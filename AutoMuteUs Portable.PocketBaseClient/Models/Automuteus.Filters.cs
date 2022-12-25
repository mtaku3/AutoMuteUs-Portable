// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (http://localhost:8090)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using PocketBaseClient.Orm.Filters;

namespace AutoMuteUs_Portable.PocketBaseClient.Models;

public partial class Automuteus
{
    public class Filters : ItemBaseFilters
    {
        /// <summary>Makes a Filter to Query data over the 'name' field</summary>
        public FilterQuery Name(OperatorText op, string value)
        {
            return FilterQuery.Create("name", op, value);
        }

        /// <summary>Makes a Filter to Query data over the 'version' field</summary>
        public FilterQuery Version(OperatorText op, string value)
        {
            return FilterQuery.Create("version", op, value);
        }

        /// <summary>Makes a Filter to Query data over the 'sha256' field</summary>
        public FilterQuery Sha256(OperatorText op, string value)
        {
            return FilterQuery.Create("sha256", op, value);
        }

        /// <summary>Makes a Filter to Query data over the 'download_url' field</summary>
        public FilterQuery DownloadUrl(OperatorText op, string value)
        {
            return FilterQuery.Create("download_url", op, value);
        }
    }
}