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

public class CollectionRedis : CollectionBase<Redis>
{
    /// <summary> Contructor: The Collection 'redis' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionRedis(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "jmpts7of6rutxjn";

    /// <inheritdoc />
    public override string Name => "redis";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'redis' </summary>
    public CollectionQuery<CollectionRedis, Redis.Sorts, Redis> Filter(Func<Redis.Filters, FilterCommand> filter)
    {
        return new(this, filter(new Redis.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'redis' </summary>
    public CollectionQuery<CollectionRedis, Redis.Sorts, Redis> All()
    {
        return new(this, null);
    }
}