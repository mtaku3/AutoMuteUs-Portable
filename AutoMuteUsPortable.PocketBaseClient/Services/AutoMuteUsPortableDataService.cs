// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (https://automuteus-portable.pockethost.io)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using AutoMuteUsPortable.PocketBaseClient.Models;
using PocketBaseClient.Services;

namespace AutoMuteUsPortable.PocketBaseClient.Services;

public class AutoMuteUsPortableDataService : DataServiceBase
{
    #region Constructor

    public AutoMuteUsPortableDataService(PocketBaseClientApplication app) : base(app)
    {
        // Collections
        ExecutorCollection = new CollectionExecutor(this);
        AutomuteusCollection = new CollectionAutomuteus(this);
        GalactusCollection = new CollectionGalactus(this);
        PostgresqlCollection = new CollectionPostgresql(this);
        RedisCollection = new CollectionRedis(this);
        AppUpdatorCollection = new CollectionAppUpdator(this);
        ApplicationCollection = new CollectionApplication(this);

        RegisterCollections();
    }

    #endregion Constructor

    #region Collections

    /// <summary> Collection 'executor' in PocketBase </summary>
    public CollectionExecutor ExecutorCollection { get; }

    /// <summary> Collection 'automuteus' in PocketBase </summary>
    public CollectionAutomuteus AutomuteusCollection { get; }

    /// <summary> Collection 'galactus' in PocketBase </summary>
    public CollectionGalactus GalactusCollection { get; }

    /// <summary> Collection 'postgresql' in PocketBase </summary>
    public CollectionPostgresql PostgresqlCollection { get; }

    /// <summary> Collection 'redis' in PocketBase </summary>
    public CollectionRedis RedisCollection { get; }

    /// <summary> Collection 'appUpdator' in PocketBase </summary>
    public CollectionAppUpdator AppUpdatorCollection { get; }

    /// <summary> Collection 'application' in PocketBase </summary>
    public CollectionApplication ApplicationCollection { get; }

    /// <inheritdoc />
    protected override void RegisterCollections()
    {
        RegisterCollection(typeof(Executor), ExecutorCollection);
        RegisterCollection(typeof(Automuteus), AutomuteusCollection);
        RegisterCollection(typeof(Galactus), GalactusCollection);
        RegisterCollection(typeof(Postgresql), PostgresqlCollection);
        RegisterCollection(typeof(Redis), RedisCollection);
        RegisterCollection(typeof(AppUpdator), AppUpdatorCollection);
        RegisterCollection(typeof(Application), ApplicationCollection);
    }

    #endregion Collections
}