// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (https://automuteus-portable.pockethost.io/)
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

public class CollectionAutomuteuses : CollectionBase<Automuteus>
{
    public CollectionAutomuteuses(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "2idyhkswler3xuj";

    /// <inheritdoc />
    public override string Name => "automuteuses";

    /// <inheritdoc />
    public override bool System => false;


    /// <summary> Query data at PocketBase, defining a Filter over collection 'automuteuses' </summary>
    public CollectionQuery<CollectionAutomuteuses, Automuteus> Filter(string filterString)
    {
        return new(this, FilterQuery.Create(filterString));
    }

    /// <summary> Query data at PocketBase, defining a Filter over collection 'automuteuses' </summary>
    public CollectionQuery<CollectionAutomuteuses, Automuteus> Filter(Func<Automuteus.Filters, FilterQuery> filter)
    {
        return new(this, filter(new Automuteus.Filters()));
    }
}