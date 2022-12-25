// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (http://localhost:8090)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using System.ComponentModel;

namespace AutoMuteUs_Portable.PocketBaseClient.Models;

public partial class Executor
{
    public enum TypeEnum
    {
        [Description("automuteus")] Automuteu,

        [Description("galactus")] Galactu,

        [Description("postgresql")] Postgresql,

        [Description("redis")] Redi
    }
}