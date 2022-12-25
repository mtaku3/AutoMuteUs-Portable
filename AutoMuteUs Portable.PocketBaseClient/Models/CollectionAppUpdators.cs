// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (http://localhost:8090)
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

namespace AutoMuteUs_Portable.PocketBaseClient.Models;

public class CollectionAppUpdators : CollectionBase<AppUpdator>
{
    public CollectionAppUpdators(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "ow8gm5ihysg7ljx";

    /// <inheritdoc />
    public override string Name => "appUpdators";

    /// <inheritdoc />
    public override bool System => false;


    /// <summary> Query data at PocketBase, defining a Filter over collection 'appUpdators' </summary>
    public CollectionQuery<CollectionAppUpdators, AppUpdator> Filter(string filterString)
    {
        return new(this, FilterQuery.Create(filterString));
    }

    /// <summary> Query data at PocketBase, defining a Filter over collection 'appUpdators' </summary>
    public CollectionQuery<CollectionAppUpdators, AppUpdator> Filter(Func<AppUpdator.Filters, FilterQuery> filter)
    {
        return new(this, filter(new AppUpdator.Filters()));
    }
}