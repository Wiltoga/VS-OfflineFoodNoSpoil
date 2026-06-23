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
    public Settings Settings => settings.Value;
    
    /// <summary>
    /// Name of the settings file
    /// </summary>
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
                    settings = Settings.Default;
                }
                else
                {
                    ValidateSettings(ref settings);
                }

                // always store the config to ensure the json has the new fields added through a new version
                server.StoreModConfig(settings, SettingsFile);
                return settings;
            }
            catch
            {
                // in case of error, reset the current settings
                server.StoreModConfig(Settings.Default, SettingsFile);
                return Settings.Default;
            }
        });
    }

    /// <summary>
    /// Validation of the fields of the settings
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
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
            // for now at least one inventory blacklist is required as the creative inventory should ALWAYS be blacklisted
            hasErrors = true;
            settings = settings with
            {
                InventoriesBlacklist = Settings.Default.InventoriesBlacklist,
            };
        }
        return hasErrors;
    }
}
