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

public partial class Checksum : ItemBase
{
    /// <inheritdoc />
    public override void UpdateWith(ItemBase itemBase)
    {
        // Do not Update with this instance
        if (ReferenceEquals(this, itemBase)) return;

        base.UpdateWith(itemBase);

        if (itemBase is Checksum item)
        {
            WinX86 = item.WinX86;
            WinX64 = item.WinX64;
            WinArm = item.WinArm;
            WinArm64 = item.WinArm64;
        }
    }

    #region Collection

    public static CollectionChecksum GetCollection()
    {
        return (CollectionChecksum)DataServiceBase.GetCollection<Checksum>()!;
    }

    #endregion Collection

    public static async Task<Checksum?> GetByIdAsync(string id, bool reload = false)
    {
        return await GetCollection().GetByIdAsync(id, reload);
    }

    public static Checksum? GetById(string id, bool reload = false)
    {
        return GetCollection().GetById(id, reload);
    }

    #region Collection

    private static CollectionBase? _Collection;

    /// <inheritdoc />
    [JsonIgnore]
    public override CollectionBase Collection => _Collection ??= DataServiceBase.GetCollection<Checksum>()!;

    #endregion Collection

    #region Field Properties

    private string? _WinX86;

    /// <summary> Maps to 'win_x86' field in PocketBase </summary>
    [JsonPropertyName("win_x86")]
    [PocketBaseField("xfeziqlb", "win_x86", false, false, false, "text")]
    [Display(Name = "Win x86")]
    [RegularExpression(
        @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$",
        ErrorMessage =
            @"Pattern '^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$' not match")]
    public string? WinX86
    {
        get => Get(() => _WinX86);
        set => Set(value, ref _WinX86);
    }

    private string? _WinX64;

    /// <summary> Maps to 'win_x64' field in PocketBase </summary>
    [JsonPropertyName("win_x64")]
    [PocketBaseField("pwf9bwxs", "win_x64", false, false, false, "text")]
    [Display(Name = "Win x64")]
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
    [PocketBaseField("qtynwwos", "win_arm", false, false, false, "text")]
    [Display(Name = "Win arm")]
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
    [PocketBaseField("go9gtjqi", "win_arm64", false, false, false, "text")]
    [Display(Name = "Win arm64")]
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

    #region Constructors

    public Checksum()
    {
    }

    [JsonConstructor]
    public Checksum(string? id, DateTime? created, DateTime? updated, string? winX86, string? winX64, string? winArm,
        string? winArm64)
        : base(id, created, updated)
    {
        WinX86 = winX86;
        WinX64 = winX64;
        WinArm = winArm;
        WinArm64 = winArm64;

        AddInternal(this);
    }

    #endregion
}