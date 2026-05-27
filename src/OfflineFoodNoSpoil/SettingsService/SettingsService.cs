using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class SettingsService : ISettingsService
{
    private readonly ICoreServerAPI server;
    private readonly ModInfo modInfo;
    private readonly Lazy<Settings> settings;
    
    internal string SettingsFile => $"{modInfo.ModID}.json";

    public SettingsService()
    {
        server = Scope.Inject<ICoreServerAPI>();
        modInfo = Scope.Inject<ModInfo>();
        settings = new(() =>
        {
            try
            {
                var settings = server.LoadModConfig<Settings>(SettingsFile);
                if (settings is null)
                {
                    server.StoreModConfig(Settings.Default, SettingsFile);
                    return Settings.Default;
                }

                ValidateSettings(ref settings);
                server.StoreModConfig(settings, SettingsFile);
                return settings;
            }
            catch
            {
                server.StoreModConfig(Settings.Default, SettingsFile);
                return Settings.Default;
            }
        });
    }

    public Settings Settings => settings.Value;

    private static bool ValidateSettings(ref Settings settings)
    {
        var hasErrors = false;
        if (settings.FoodSpoilMultiplier is float.NaN)
        {
            hasErrors = true;
            settings = settings with
            {
                FoodSpoilMultiplier = Settings.Default.FoodSpoilMultiplier,
            };
        }
        if (settings.FoodSpoilMultiplier is < 0 or > 1)
        {
            hasErrors = true;
            settings = settings with
            {
                FoodSpoilMultiplier = Math.Clamp(settings.FoodSpoilMultiplier, 0, 1),
            };
        }

        if (settings.MaxAllowedSkippedHours is float.NaN or < 0)
        {
            hasErrors = true;
            settings = settings with
            {
                MaxAllowedSkippedHours = null,
            };
        }

        if (settings.InventoriesBlacklist?.Contains(null) is true)
        {
            hasErrors = true;
            settings = settings with
            {
                InventoriesBlacklist = [.. settings.InventoriesBlacklist.Where(inventoryClass => inventoryClass is not null)],
            };
        }
        if (settings.InventoriesBlacklist?.Length is not > 0)
        {
            hasErrors = true;
            settings = settings with
            {
                InventoriesBlacklist = Settings.Default.InventoriesBlacklist,
            };
        }
        return hasErrors;
    }
}
