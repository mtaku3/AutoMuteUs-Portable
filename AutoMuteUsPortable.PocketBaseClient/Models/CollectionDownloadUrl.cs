// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (https://automuteus-portable.pockethost.io)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using PocketBaseClient.Orm;
using PocketBaseClient.Orm.Filters;
using PocketBaseClient.Services;

namespace AutoMuteUsPortable.PocketBaseClient.Models;

public class CollectionDownloadUrl : CollectionBase<DownloadUrl>
{
    /// <summary> Contructor: The Collection 'downloadUrl' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionDownloadUrl(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "w6u17m2jdh3pu7w";

    /// <inheritdoc />
    public override string Name => "downloadUrl";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'downloadUrl' </summary>
    public CollectionQuery<CollectionDownloadUrl, DownloadUrl.Sorts, DownloadUrl> Filter(
        Func<DownloadUrl.Filters, FilterCommand> filter)
    {
        return new(this, filter(new DownloadUrl.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'downloadUrl' </summary>
    public CollectionQuery<CollectionDownloadUrl, DownloadUrl.Sorts, DownloadUrl> All()
    {
        return new(this, null);
    }
}