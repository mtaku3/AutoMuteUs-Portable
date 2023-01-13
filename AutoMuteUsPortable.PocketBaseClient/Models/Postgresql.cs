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

public partial class Postgresql : ItemBase
{
    /// <inheritdoc />
    protected override IEnumerable<ItemBase?> RelatedItems
        => base.RelatedItems.Union(CompatibleExecutors);

    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        base.UpdateWith(itemBase);

        if (itemBase is Postgresql item)
        {
            Version = item.Version;
            Hashes = item.Hashes;
            DownloadUrl = item.DownloadUrl;
            CompatibleExecutors = item.CompatibleExecutors;
        }
    }

    #region Collection

    public static CollectionPostgresql GetCollection()
    {
        return (CollectionPostgresql)DataServiceBase.GetCollection<Postgresql>()!;
    }

    #endregion Collection

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<Postgresql>()!;

    #endregion Collection

    #region Field Properties

    private string? _Version;

    /// <summary> Maps to 'version' field in PocketBase </summary>
    [JsonPropertyName("version")]
    [PocketBaseField("zrg75eu6", "version", true, false, true, "text")]
    [Display(Name = "Version")]
    [Required(ErrorMessage = @"Version is required")]
    public string? Version
    {
        get => Get(() => _Version);
        set => Set(value, ref _Version);
    }

    private string? _Hashes;

    /// <summary> Maps to 'hashes' field in PocketBase </summary>
    [JsonPropertyName("hashes")]
    [PocketBaseField("2x73jgyu", "hashes", false, false, false, "text")]
    [Display(Name = "Hashes")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? Hashes
    {
        get => Get(() => _Hashes);
        set => Set(value, ref _Hashes);
    }

    private string? _DownloadUrl;

    /// <summary> Maps to 'download_url' field in PocketBase </summary>
    [JsonPropertyName("download_url")]
    [PocketBaseField("3gquzcrh", "download_url", false, false, false, "text")]
    [Display(Name = "Download_url")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? DownloadUrl
    {
        get => Get(() => _DownloadUrl);
        set => Set(value, ref _DownloadUrl);
    }

    private CompatibleExecutorsList _CompatibleExecutors = new();

    /// <summary> Maps to 'compatibleExecutors' field in PocketBase </summary>
    [JsonPropertyName("compatibleExecutors")]
    [PocketBaseField("4d6o86dl", "compatibleExecutors", false, false, false, "relation")]
    [Display(Name = "Compatible Executors")]
    [JsonInclude]
    [JsonConverter(typeof(RelationListConverter<CompatibleExecutorsList, Executor>))]
    public CompatibleExecutorsList CompatibleExecutors
    {
        get => Get(() => _CompatibleExecutors ??= new CompatibleExecutorsList(this));
        private set => Set(value, ref _CompatibleExecutors);
    }

    #endregion Field Properties

    #region GetById

    public static Postgresql? GetById(string id, bool reload = false)
    {
        return GetByIdAsync(id, reload).Result;
    }

    public static async Task<Postgresql?> GetByIdAsync(string id, bool reload = false)
    {
        return await DataServiceBase.GetCollection<Postgresql>()!.GetByIdAsync(id, reload);
    }

    #endregion GetById
}