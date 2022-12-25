// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (http://localhost:8090)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using PocketBaseClient.Orm.Structures;

namespace AutoMuteUs_Portable.PocketBaseClient.Models;

public partial class Redis
{
    public class CompatibleExecutorsList : FieldItemList<Executor>
    {
        public CompatibleExecutorsList() : this(null)
        {
        }

        public CompatibleExecutorsList(Redis? redis) : base(redis, "CompatibleExecutors", "19esueem")
        {
        }
    }
}