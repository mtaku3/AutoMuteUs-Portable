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

public class CollectionChecksum : CollectionBase<Checksum>
{
    /// <summary> Contructor: The Collection 'checksum' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionChecksum(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "tegy62vbkxgbnp3";

    /// <inheritdoc />
    public override string Name => "checksum";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'checksum' </summary>
    public CollectionQuery<CollectionChecksum, Checksum.Sorts, Checksum> Filter(
        Func<Checksum.Filters, FilterCommand> filter)
    {
        return new(this, filter(new Checksum.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'checksum' </summary>
    public CollectionQuery<CollectionChecksum, Checksum.Sorts, Checksum> All()
    {
        return new(this, null);
    }
}