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

public class CollectionGalactus : CollectionBase<Galactus>
{
    /// <summary> Contructor: The Collection 'galactus' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionGalactus(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "ki4t07iokc8f3eg";

    /// <inheritdoc />
    public override string Name => "galactus";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'galactus' </summary>
    public CollectionQuery<CollectionGalactus, Galactus.Sorts, Galactus> Filter(
        Func<Galactus.Filters, FilterCommand> filter)
    {
        return new(this, filter(new Galactus.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'galactus' </summary>
    public CollectionQuery<CollectionGalactus, Galactus.Sorts, Galactus> All()
    {
        return new(this, null);
    }
}