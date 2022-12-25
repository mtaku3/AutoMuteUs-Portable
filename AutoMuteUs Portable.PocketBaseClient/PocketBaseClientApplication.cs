// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (http://localhost:8090)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using AutoMuteUs_Portable.PocketBaseClient.Services;

namespace AutoMuteUs_Portable.PocketBaseClient;

public class PocketBaseClientApplication : global::PocketBaseClient.PocketBaseClientApplication
{
    private AutoMuteUsPortableDataService? _Data;

    /// <summary> Access to Data for Application AutoMuteUs Portable </summary>
    public AutoMuteUsPortableDataService Data => _Data ??= new AutoMuteUsPortableDataService(this);

    #region Constructors

    public PocketBaseClientApplication() : this("https://automuteus-portable.pockethost.io/")
    {
    }

    public PocketBaseClientApplication(string url, string appName = "AutoMuteUs Portable") : base(url, appName)
    {
    }

    #endregion Constructors
}