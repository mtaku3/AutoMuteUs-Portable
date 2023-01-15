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

public partial class Executor : ItemBase
{
    /// <inheritdoc />
    protected override IEnumerable<ItemBase?> RelatedItems
        => base.RelatedItems.Union(new List<ItemBase?> { DownloadUrl });

    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        base.UpdateWith(itemBase);

        if (itemBase is Executor item)
        {
            Version = item.Version;
            Type = item.Type;
            DownloadUrl = item.DownloadUrl;
        }
    }

    #region Collection

    public static CollectionExecutor GetCollection()
    {
        return (CollectionExecutor)DataServiceBase.GetCollection<Executor>()!;
    }

    #endregion Collection

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<Executor>()!;

    #endregion Collection

    #region Field Properties

    private string? _Version;

    /// <summary> Maps to 'version' field in PocketBase </summary>
    [JsonPropertyName("version")]
    [PocketBaseField("hwcfmqju", "version", true, false, true, "text")]
    [Display(Name = "Version")]
    [Required(ErrorMessage = @"Version is required")]
    public string? Version
    {
        get => Get(() => _Version);
        set => Set(value, ref _Version);
    }

    private TypeEnum? _Type;

    /// <summary> Maps to 'type' field in PocketBase </summary>
    [JsonPropertyName("type")]
    [PocketBaseField("psgibu4y", "type", true, false, false, "select")]
    [Display(Name = "Type")]
    [Required(ErrorMessage = @"Type is required")]
    [JsonConverter(typeof(EnumConverter<TypeEnum>))]
    public TypeEnum? Type
    {
        get => Get(() => _Type);
        set => Set(value, ref _Type);
    }

    private DownloadUrl? _DownloadUrl;

    /// <summary> Maps to 'download_url' field in PocketBase </summary>
    [JsonPropertyName("download_url")]
    [PocketBaseField("b5txfun2", "download_url", false, false, false, "relation")]
    [Display(Name = "Download_url")]
    [JsonConverter(typeof(RelationConverter<DownloadUrl>))]
    public DownloadUrl? DownloadUrl
    {
        get => Get(() => _DownloadUrl);
        set => Set(value, ref _DownloadUrl);
    }

    #endregion Field Properties

    #region GetById

    public static Executor? GetById(string id, bool reload = false)
    {
        return GetByIdAsync(id, reload).Result;
    }

    public static async Task<Executor?> GetByIdAsync(string id, bool reload = false)
    {
        return await DataServiceBase.GetCollection<Executor>()!.GetByIdAsync(id, reload);
    }

    #endregion GetById
}