// This file was generated automatically for the PocketBase Application AutoMuteUs Portable (http://localhost:8090)
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

namespace AutoMuteUs_Portable.PocketBaseClient.Models;

public partial class Application : ItemBase
{
    /// <inheritdoc />
    protected override IEnumerable<ItemBase?> RelatedItems
        => base.RelatedItems.Union(new List<ItemBase?> { AppUpdator }).Union(CompatibleExecutors);

    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        base.UpdateWith(itemBase);

        if (itemBase is Application item)
        {
            Name = item.Name;
            Version = item.Version;
            Sha256 = item.Sha256;
            DownloadUrl = item.DownloadUrl;
            AppUpdator = item.AppUpdator;
            CompatibleExecutors = item.CompatibleExecutors;
        }
    }

    #region Collection

    public static CollectionApplications GetCollection()
    {
        return (CollectionApplications)DataServiceBase.GetCollection<Application>()!;
    }

    #endregion Collection

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<Application>()!;

    #endregion Collection

    #region Field Properties

    private string? _Name;

    /// <summary> Maps to 'name' field in PocketBase </summary>
    [JsonPropertyName("name")]
    [PocketBaseField("vofsjb0u", "name", true, false, true, "text")]
    [Display(Name = "Name")]
    [Required(ErrorMessage = @"name is required")]
    public string? Name
    {
        get => Get(() => _Name);
        set => Set(value, ref _Name);
    }

    private string? _Version;

    /// <summary> Maps to 'version' field in PocketBase </summary>
    [JsonPropertyName("version")]
    [PocketBaseField("gaodbfln", "version", true, false, true, "text")]
    [Display(Name = "Version")]
    [Required(ErrorMessage = @"version is required")]
    public string? Version
    {
        get => Get(() => _Version);
        set => Set(value, ref _Version);
    }

    private string? _Sha256;

    /// <summary> Maps to 'sha256' field in PocketBase </summary>
    [JsonPropertyName("sha256")]
    [PocketBaseField("zrrbclvi", "sha256", true, false, false, "text")]
    [Display(Name = "Sha256")]
    [Required(ErrorMessage = @"sha256 is required")]
    [RegularExpression(@"^[A-Fa-f0-9]{64}$", ErrorMessage = @"Pattern '^[A-Fa-f0-9]{64}$' not match")]
    public string? Sha256
    {
        get => Get(() => _Sha256);
        set => Set(value, ref _Sha256);
    }

    private string? _DownloadUrl;

    /// <summary> Maps to 'download_url' field in PocketBase </summary>
    [JsonPropertyName("download_url")]
    [PocketBaseField("kwvvdz4b", "download_url", true, false, false, "text")]
    [Display(Name = "Download_url")]
    [Required(ErrorMessage = @"download_url is required")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? DownloadUrl
    {
        get => Get(() => _DownloadUrl);
        set => Set(value, ref _DownloadUrl);
    }

    private AppUpdator? _AppUpdator;

    /// <summary> Maps to 'appUpdator' field in PocketBase </summary>
    [JsonPropertyName("appUpdator")]
    [PocketBaseField("pugxnk1r", "appUpdator", true, false, true, "relation")]
    [Display(Name = "App Updator")]
    [Required(ErrorMessage = @"appUpdator is required")]
    [JsonConverter(typeof(RelationConverter<AppUpdator>))]
    public AppUpdator? AppUpdator
    {
        get => Get(() => _AppUpdator);
        set => Set(value, ref _AppUpdator);
    }

    private CompatibleExecutorsList _CompatibleExecutors = new();

    /// <summary> Maps to 'compatibleExecutors' field in PocketBase </summary>
    [JsonPropertyName("compatibleExecutors")]
    [PocketBaseField("bmowby1z", "compatibleExecutors", true, false, false, "relation")]
    [Display(Name = "Compatible Executors")]
    [Required(ErrorMessage = @"compatibleExecutors is required")]
    [JsonConverter(typeof(RelationListConverter<CompatibleExecutorsList, Executor>))]
    public CompatibleExecutorsList CompatibleExecutors
    {
        get => Get(() => _CompatibleExecutors ??= new CompatibleExecutorsList(this));
        private set => Set(value, ref _CompatibleExecutors);
    }

    #endregion Field Properties


    #region GetById

    public static Application? GetById(string id, bool reload = false)
    {
        return GetByIdAsync(id, reload).Result;
    }

    public static async Task<Application?> GetByIdAsync(string id, bool reload = false)
    {
        return await DataServiceBase.GetCollection<Application>()!.GetByIdAsync(id, reload);
    }

    #endregion GetById
}