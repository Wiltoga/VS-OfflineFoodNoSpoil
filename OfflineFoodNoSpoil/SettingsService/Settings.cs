namespace Wiltoga.OfflineFoodNoSpoil;

public record Settings
{
    public bool EnableMod { get; init; } = true;
    public bool UseLogs { get; init; } = false;
    public float FoodSpoilMultiplier { get; init; } = 0f;
    public float? MaxAllowedSkippedHours { get; init; } = null;
    public string[] InventoriesBlacklist { get; init; } = ["creative"];
    public static Settings Default { get; } = new();
}
