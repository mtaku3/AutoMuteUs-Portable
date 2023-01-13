// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (https://automuteus-portable.pockethost.io)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using PocketBaseClient.Orm.Structures;

namespace AutoMuteUsPortable.PocketBaseClient.Models;

public partial class Automuteus
{
    public class CompatibleExecutorsList : FieldItemList<Executor>
    {
        public CompatibleExecutorsList() : this(null)
        {
        }

        public CompatibleExecutorsList(Automuteus? automuteus) : base(automuteus, "CompatibleExecutors", "ovmgcceo")
        {
        }
    }
}