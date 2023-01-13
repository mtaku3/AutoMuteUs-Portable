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

public class CollectionAutomuteus : CollectionBase<Automuteus>
{
    /// <summary> Contructor: The Collection 'automuteus' </summary>
    /// <param name="context">The DataService for the collection</param>
    internal CollectionAutomuteus(DataServiceBase context) : base(context)
    {
    }

    /// <inheritdoc />
    public override string Id => "2idyhkswler3xuj";

    /// <inheritdoc />
    public override string Name => "automuteus";

    /// <inheritdoc />
    public override bool System => false;

    /// <summary> Query data at PocketBase, defining a Filter over collection 'automuteus' </summary>
    public CollectionQuery<CollectionAutomuteus, Automuteus.Sorts, Automuteus> Filter(
        Func<Automuteus.Filters, FilterCommand> filter)
    {
        return new(this, filter(new Automuteus.Filters()));
    }

    /// <summary> Query all data at PocketBase, over collection 'automuteus' </summary>
    public CollectionQuery<CollectionAutomuteus, Automuteus.Sorts, Automuteus> All()
    {
        return new(this, null);
    }
}