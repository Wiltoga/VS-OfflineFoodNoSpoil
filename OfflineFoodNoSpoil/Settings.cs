namespace OfflineFoodNoSpoil;

public class Settings
{
    public bool EnableMod { get; set; }
    public bool UseLogs { get; set; }
    public float FoodSpoilMultiplier { get; set; }

    public static Settings Default => new() { EnableMod = true, UseLogs = true, FoodSpoilMultiplier = 0f };
}
