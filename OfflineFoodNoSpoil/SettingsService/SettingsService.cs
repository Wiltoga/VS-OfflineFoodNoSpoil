using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class SettingsService : ISettingsService
{
    private readonly ICoreServerAPI server;
    private readonly Mod mod;
    private readonly Lazy<Settings> settings;
    
    private string SettingsFile => $"{mod.Info.ModID}.json";

    public SettingsService()
    {
        server = Scope.Inject<ICoreServerAPI>();
        mod = Scope.Inject<Mod>();
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
                else
                {
                    // force update to new version, so that it includes all new fields
                    server.StoreModConfig(settings, SettingsFile);
                }
                var hasErrors = false;

                if (settings.FoodSpoilMultiplier < 0 || settings.FoodSpoilMultiplier > 1)
                {
                    hasErrors = true;
                    settings.FoodSpoilMultiplier = Math.Clamp(settings.FoodSpoilMultiplier, 0, 1);
                }
                if (settings.MaxAllowedSkippedHours < 0)
                {
                    hasErrors = true;
                    settings.MaxAllowedSkippedHours = null;
                }
                if (settings.InventoriesBlacklist is null || settings.InventoriesBlacklist.Length == 0)
                {
                    hasErrors = true;
                    settings.InventoriesBlacklist = Settings.Default.InventoriesBlacklist;
                }
                if (hasErrors)
                {
                    server.StoreModConfig(settings, SettingsFile);
                }
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
}
