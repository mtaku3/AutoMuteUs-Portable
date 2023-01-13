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

public class CollectionAppUpdator : CollectionBase<AppUpdator>
{
    /// <summary> Contructor: The Collection 'appUpdator' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionAppUpdator(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "ow8gm5ihysg7ljx";

    /// <inheritdoc />
    public override string Name => "appUpdator";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'appUpdator' </summary>
    public CollectionQuery<CollectionAppUpdator, AppUpdator.Sorts, AppUpdator> Filter(
        Func<AppUpdator.Filters, FilterCommand> filter)
    {
        return new(this, filter(new AppUpdator.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'appUpdator' </summary>
    public CollectionQuery<CollectionAppUpdator, AppUpdator.Sorts, AppUpdator> All()
    {
        return new(this, null);
    }
}