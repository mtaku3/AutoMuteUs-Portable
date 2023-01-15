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
using PocketBaseClient.Services;

namespace AutoMuteUsPortable.PocketBaseClient.Models;

public partial class DownloadUrl : ItemBase
{
    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        base.UpdateWith(itemBase);

        if (itemBase is DownloadUrl item)
        {
            Win86 = item.Win86;
            WinX64 = item.WinX64;
            WinArm = item.WinArm;
            WinArm64 = item.WinArm64;
        }
    }

    #region Collection

    public static CollectionDownloadUrl GetCollection()
    {
        return (CollectionDownloadUrl)DataServiceBase.GetCollection<DownloadUrl>()!;
    }

    #endregion Collection

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<DownloadUrl>()!;

    #endregion Collection

    #region Field Properties

    private string? _Win86;

    /// <summary> Maps to 'win_86' field in PocketBase </summary>
    [JsonPropertyName("win_86")]
    [PocketBaseField("dh3xuogj", "win_86", false, false, false, "text")]
    [Display(Name = "Win_86")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? Win86
    {
        get => Get(() => _Win86);
        set => Set(value, ref _Win86);
    }

    private string? _WinX64;

    /// <summary> Maps to 'win_x64' field in PocketBase </summary>
    [JsonPropertyName("win_x64")]
    [PocketBaseField("cl201qm7", "win_x64", false, false, false, "text")]
    [Display(Name = "Win_x64")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? WinX64
    {
        get => Get(() => _WinX64);
        set => Set(value, ref _WinX64);
    }

    private string? _WinArm;

    /// <summary> Maps to 'win_arm' field in PocketBase </summary>
    [JsonPropertyName("win_arm")]
    [PocketBaseField("hkf0q4kd", "win_arm", false, false, false, "text")]
    [Display(Name = "Win_arm")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? WinArm
    {
        get => Get(() => _WinArm);
        set => Set(value, ref _WinArm);
    }

    private string? _WinArm64;

    /// <summary> Maps to 'win_arm64' field in PocketBase </summary>
    [JsonPropertyName("win_arm64")]
    [PocketBaseField("2oicknyy", "win_arm64", false, false, false, "text")]
    [Display(Name = "Win_arm64")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? WinArm64
    {
        get => Get(() => _WinArm64);
        set => Set(value, ref _WinArm64);
    }

    #endregion Field Properties

    #region GetById

    public static DownloadUrl? GetById(string id, bool reload = false)
    {
        return GetByIdAsync(id, reload).Result;
    }

    public static async Task<DownloadUrl?> GetByIdAsync(string id, bool reload = false)
    {
        return await DataServiceBase.GetCollection<DownloadUrl>()!.GetByIdAsync(id, reload);
    }

    #endregion GetById
}