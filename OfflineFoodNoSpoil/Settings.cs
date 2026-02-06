namespace Wiltoga.OfflineFoodNoSpoil;

public class Settings
{
    public bool EnableMod { get; set; } = true;
    public bool UseLogs { get; set; } = false;
    public float FoodSpoilMultiplier { get; set; } = 0f;
    public float? MaxAllowedSkippedHours { get; set; } = null;
    public static Settings Default => new();
}
