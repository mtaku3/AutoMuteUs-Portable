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

public partial class AppUpdator : ItemBase
{
    /// <inheritdoc />
    protected override IEnumerable<ItemBase?> RelatedItems
        => base.RelatedItems.Union(new List<ItemBase?> { DownloadUrl }).Union(new List<ItemBase?> { Checksum });

    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        base.UpdateWith(itemBase);

        if (itemBase is AppUpdator item)
        {
            Version = item.Version;
            DownloadUrl = item.DownloadUrl;
            Checksum = item.Checksum;
        }
    }

    #region Collection

    public static CollectionAppUpdator GetCollection()
    {
        return (CollectionAppUpdator)DataServiceBase.GetCollection<AppUpdator>()!;
    }

    #endregion Collection

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<AppUpdator>()!;

    #endregion Collection

    #region Field Properties

    private string? _Version;

    /// <summary> Maps to 'version' field in PocketBase </summary>
    [JsonPropertyName("version")]
    [PocketBaseField("svgrplz4", "version", true, false, true, "text")]
    [Display(Name = "Version")]
    [Required(ErrorMessage = @"Version is required")]
    public string? Version
    {
        get => Get(() => _Version);
        set => Set(value, ref _Version);
    }

    private DownloadUrl? _DownloadUrl;

    /// <summary> Maps to 'download_url' field in PocketBase </summary>
    [JsonPropertyName("download_url")]
    [PocketBaseField("f2fcbhx6", "download_url", false, false, false, "relation")]
    [Display(Name = "Download url")]
    [JsonConverter(typeof(RelationConverter<DownloadUrl>))]
    public DownloadUrl? DownloadUrl
    {
        get => Get(() => _DownloadUrl);
        set => Set(value, ref _DownloadUrl);
    }

    private Checksum? _Checksum;

    /// <summary> Maps to 'checksum' field in PocketBase </summary>
    [JsonPropertyName("checksum")]
    [PocketBaseField("3r6kfxra", "checksum", false, false, false, "relation")]
    [Display(Name = "Checksum")]
    [JsonConverter(typeof(RelationConverter<Checksum>))]
    public Checksum? Checksum
    {
        get => Get(() => _Checksum);
        set => Set(value, ref _Checksum);
    }

    #endregion Field Properties

    #region GetById

    public static AppUpdator? GetById(string id, bool reload = false)
    {
        return Task.Run(async () => await GetByIdAsync(id, reload)).GetAwaiter().GetResult();
    }

    public static async Task<AppUpdator?> GetByIdAsync(string id, bool reload = false)
    {
        return await DataServiceBase.GetCollection<AppUpdator>()!.GetByIdAsync(id, reload);
    }

    #endregion GetById
}