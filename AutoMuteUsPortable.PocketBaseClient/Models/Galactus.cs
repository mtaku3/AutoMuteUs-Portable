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

public partial class Galactus : ItemBase
{
    /// <inheritdoc />
    protected override IEnumerable<ItemBase?> RelatedItems
        => base.RelatedItems.Union(CompatibleExecutors).Union(new List<ItemBase?> { DownloadUrl })
            .Union(new List<ItemBase?> { Checksum });

    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        base.UpdateWith(itemBase);

        if (itemBase is Galactus item)
        {
            Version = item.Version;
            CompatibleExecutors = item.CompatibleExecutors;
            DownloadUrl = item.DownloadUrl;
            Checksum = item.Checksum;
        }
    }

    #region Collection

    public static CollectionGalactus GetCollection()
    {
        return (CollectionGalactus)DataServiceBase.GetCollection<Galactus>()!;
    }

    #endregion Collection

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<Galactus>()!;

    #endregion Collection

    #region Field Properties

    private string? _Version;

    /// <summary> Maps to 'version' field in PocketBase </summary>
    [JsonPropertyName("version")]
    [PocketBaseField("bqq3r23t", "version", true, false, true, "text")]
    [Display(Name = "Version")]
    [Required(ErrorMessage = @"Version is required")]
    public string? Version
    {
        get => Get(() => _Version);
        set => Set(value, ref _Version);
    }

    private CompatibleExecutorsList _CompatibleExecutors = new();

    /// <summary> Maps to 'compatibleExecutors' field in PocketBase </summary>
    [JsonPropertyName("compatibleExecutors")]
    [PocketBaseField("owy480ko", "compatibleExecutors", false, false, false, "relation")]
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
    [PocketBaseField("jnieamng", "download_url", false, false, false, "relation")]
    [Display(Name = "Download_url")]
    [JsonConverter(typeof(RelationConverter<DownloadUrl>))]
    public DownloadUrl? DownloadUrl
    {
        get => Get(() => _DownloadUrl);
        set => Set(value, ref _DownloadUrl);
    }

    private Checksum? _Checksum;

    /// <summary> Maps to 'checksum' field in PocketBase </summary>
    [JsonPropertyName("checksum")]
    [PocketBaseField("ci0voncf", "checksum", false, false, false, "relation")]
    [Display(Name = "Checksum")]
    [JsonConverter(typeof(RelationConverter<Checksum>))]
    public Checksum? Checksum
    {
        get => Get(() => _Checksum);
        set => Set(value, ref _Checksum);
    }

    #endregion Field Properties

    #region GetById

    public static Galactus? GetById(string id, bool reload = false)
    {
        return GetByIdAsync(id, reload).Result;
    }

    public static async Task<Galactus?> GetByIdAsync(string id, bool reload = false)
    {
        return await DataServiceBase.GetCollection<Galactus>()!.GetByIdAsync(id, reload);
    }

    #endregion GetById
}