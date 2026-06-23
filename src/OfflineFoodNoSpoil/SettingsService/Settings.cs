namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Mod settings
/// </summary>
public sealed record Settings
{
    /// <summary>
    /// If the mod is enabled at all
    /// </summary>
    public bool EnableMod { get; init; } = true;
    
    /// <summary>
    /// If the mod should log in DEBUG
    /// </summary>
    public bool UseLogs { get; init; } = false;

    /// <summary>
    /// If the mod should create crash dumps on error
    /// </summary>
    public bool CreateCrashDumps { get; init; } = true;

    /// <summary>
    /// The multiplier of the food spoil time while offline.
    /// </summary>
    /// <remarks>
    /// 0 means no spoiling <br />
    /// 1 means the spoiling time stays unchanged
    /// </remarks>
    public float FoodSpoilMultiplier { get; init; } = 0f;

    /// <summary>
    /// If defined, clamps the skipped spoil time while offline by this amount in hours
    /// </summary>
    public float? MaxAllowedSkippedHours { get; init; } = null;

    /// <summary>
    /// List of inventory classes to not handle
    /// </summary>
    public string[] InventoriesBlacklist { get; init; } = ["creative"];

    /// <summary>
    /// The default configuration of the settings
    /// </summary>
    public static Settings Default { get; } = new();
}
