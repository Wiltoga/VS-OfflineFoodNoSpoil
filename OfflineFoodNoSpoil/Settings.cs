namespace Wiltoga.OfflineFoodNoSpoil;

public class Settings
{
    public bool EnableMod { get; set; }
    public bool UseLogs { get; set; }
    public float FoodSpoilMultiplier { get; set; }
    public float? MaxAllowedSkippedHours { get; set; }
    public static Settings Default => new() { EnableMod = true, UseLogs = false, FoodSpoilMultiplier = 0f, MaxAllowedSkippedHours = null };
}
