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

public class CollectionPostgresql : CollectionBase<Postgresql>
{
    /// <summary> Contructor: The Collection 'postgresql' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionPostgresql(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "a7yc5nczru892st";

    /// <inheritdoc />
    public override string Name => "postgresql";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'postgresql' </summary>
    public CollectionQuery<CollectionPostgresql, Postgresql.Sorts, Postgresql> Filter(
        Func<Postgresql.Filters, FilterCommand> filter)
    {
        return new(this, filter(new Postgresql.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'postgresql' </summary>
    public CollectionQuery<CollectionPostgresql, Postgresql.Sorts, Postgresql> All()
    {
        return new(this, null);
    }
}