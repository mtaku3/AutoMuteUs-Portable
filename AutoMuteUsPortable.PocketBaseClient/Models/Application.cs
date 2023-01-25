// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (https://automuteus-portable.pockethost.io)
//    See CodeGenerationSummary.txt for more details
//
// PocketBaseClient-csharp project: https://github.com/iluvadev/PocketBaseClient-csharp
// Issues: https://github.com/iluvadev/PocketBaseClient-csharp/issues
// License (MIT): https://github.com/iluvadev/PocketBaseClient-csharp/blob/main/LICENSE
//
// pocketbase-csharp-sdk project: https://github.com/PRCV1/pocketbase-csharp-sdk 
// pocketbase project: https://github.com/pocketbase/pocketbase

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PocketBaseClient.Orm;
using PocketBaseClient.Orm.Attributes;
using PocketBaseClient.Orm.Json;
using PocketBaseClient.Services;

namespace AutoMuteUsPortable.PocketBaseClient.Models;

public partial class Application : ItemBase
{
    /// <inheritdoc />
    protected override IEnumerable<ItemBase?> RelatedItems
        => base.RelatedItems.Union(new List<ItemBase?> { AppUpdator }).Union(CompatibleExecutors)
            .Union(new List<ItemBase?> { DownloadUrl });

    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        base.UpdateWith(itemBase);

        if (itemBase is Application item)
        {
            Version = item.Version;
            AppUpdator = item.AppUpdator;
            CompatibleExecutors = item.CompatibleExecutors;
            DownloadUrl = item.DownloadUrl;
        }
    }

    #region Collection

    public static CollectionApplication GetCollection()
    {
        return (CollectionApplication)DataServiceBase.GetCollection<Application>()!;
    }

    #endregion Collection

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<Application>()!;

    #endregion Collection

    #region Field Properties

    private string? _Version;

    /// <summary> Maps to 'version' field in PocketBase </summary>
    [JsonPropertyName("version")]
    [PocketBaseField("gaodbfln", "version", true, false, true, "text")]
    [Display(Name = "Version")]
    [Required(ErrorMessage = @"Version is required")]
    public string? Version
    {
        get => Get(() => _Version);
        set => Set(value, ref _Version);
    }

    private AppUpdator? _AppUpdator;

    /// <summary> Maps to 'appUpdator' field in PocketBase </summary>
    [JsonPropertyName("appUpdator")]
    [PocketBaseField("pugxnk1r", "appUpdator", false, false, true, "relation")]
    [Display(Name = "App Updator")]
    [JsonConverter(typeof(RelationConverter<AppUpdator>))]
    public AppUpdator? AppUpdator
    {
        get => Get(() => _AppUpdator);
        set => Set(value, ref _AppUpdator);
    }

    private CompatibleExecutorsList _CompatibleExecutors = new();

    /// <summary> Maps to 'compatibleExecutors' field in PocketBase </summary>
    [JsonPropertyName("compatibleExecutors")]
    [PocketBaseField("bmowby1z", "compatibleExecutors", false, false, false, "relation")]
    [Display(Name = "Compatible Executors")]
    [JsonInclude]
    [JsonConverter(typeof(RelationListConverter<CompatibleExecutorsList, Executor>))]
    public CompatibleExecutorsList CompatibleExecutors
    {
        get => Get(() => _CompatibleExecutors ??= new CompatibleExecutorsList(this));
        private set => Set(value, ref _CompatibleExecutors);
    }

    private DownloadUrl? _DownloadUrl;

    /// <summary> Maps to 'download_url' field in PocketBase </summary>
    [JsonPropertyName("download_url")]
    [PocketBaseField("gtc97rof", "download_url", false, false, false, "relation")]
    [Display(Name = "Download url")]
    [JsonConverter(typeof(RelationConverter<DownloadUrl>))]
    public DownloadUrl? DownloadUrl
    {
        get => Get(() => _DownloadUrl);
        set => Set(value, ref _DownloadUrl);
    }

    #endregion Field Properties

    #region GetById

    public static Application? GetById(string id, bool reload = false)
    {
        return Task.Run(async () => await GetByIdAsync(id, reload)).GetAwaiter().GetResult();
    }

    public static async Task<Application?> GetByIdAsync(string id, bool reload = false)
    {
        return await DataServiceBase.GetCollection<Application>()!.GetByIdAsync(id, reload);
    }

    #endregion GetById
}