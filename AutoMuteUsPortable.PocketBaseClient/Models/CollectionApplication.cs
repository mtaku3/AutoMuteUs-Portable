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

public class CollectionApplication : CollectionBase<Application>
{
    /// <summary> Contructor: The Collection 'application' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionApplication(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "76t87z4iku0886q";

    /// <inheritdoc />
    public override string Name => "application";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'application' </summary>
    public CollectionQuery<CollectionApplication, Application.Sorts, Application> Filter(
        Func<Application.Filters, FilterCommand> filter)
    {
        return new(this, filter(new Application.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'application' </summary>
    public CollectionQuery<CollectionApplication, Application.Sorts, Application> All()
    {
        return new(this, null);
    }
}