// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (https://automuteus-portable.pockethost.io/)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using AutoMuteUs_Portable.PocketBaseClient.Models;
using PocketBaseClient.Services;

namespace AutoMuteUs_Portable.PocketBaseClient.Services;

public class AutoMuteUsPortableDataService : DataServiceBase
{
    #region Constructor

    public AutoMuteUsPortableDataService(PocketBaseClientApplication app) : base(app)
    {
        // Collections
        ExecutorsCollection = new CollectionExecutors(this);
        AutomuteusesCollection = new CollectionAutomuteuses(this);
        GalactusesCollection = new CollectionGalactuses(this);
        PostgresqlsCollection = new CollectionPostgresqls(this);
        RedisesCollection = new CollectionRedises(this);
        AppUpdatorsCollection = new CollectionAppUpdators(this);
        ApplicationsCollection = new CollectionApplications(this);

        RegisterCollections();
    }

    #endregion Constructor

    #region Collections

    /// <summary> Collection 'executors' in PocketBase </summary>
    public CollectionExecutors ExecutorsCollection { get; }

    /// <summary> Collection 'automuteuses' in PocketBase </summary>
    public CollectionAutomuteuses AutomuteusesCollection { get; }

    /// <summary> Collection 'galactuses' in PocketBase </summary>
    public CollectionGalactuses GalactusesCollection { get; }

    /// <summary> Collection 'postgresqls' in PocketBase </summary>
    public CollectionPostgresqls PostgresqlsCollection { get; }

    /// <summary> Collection 'redises' in PocketBase </summary>
    public CollectionRedises RedisesCollection { get; }

    /// <summary> Collection 'appUpdators' in PocketBase </summary>
    public CollectionAppUpdators AppUpdatorsCollection { get; }

    /// <summary> Collection 'applications' in PocketBase </summary>
    public CollectionApplications ApplicationsCollection { get; }

    /// <inheritdoc />
    protected override void RegisterCollections()
    {
        RegisterCollection(typeof(Executor), ExecutorsCollection);
        RegisterCollection(typeof(Automuteus), AutomuteusesCollection);
        RegisterCollection(typeof(Galactus), GalactusesCollection);
        RegisterCollection(typeof(Postgresql), PostgresqlsCollection);
        RegisterCollection(typeof(Redis), RedisesCollection);
        RegisterCollection(typeof(AppUpdator), AppUpdatorsCollection);
        RegisterCollection(typeof(Application), ApplicationsCollection);
    }

    #endregion Collections
}