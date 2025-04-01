using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

public class OfflineFoodNoSpoil : ModSystem
{
    public ICoreServerAPI Server { get; private set; } = default!;
    public static OfflineFoodNoSpoil Instance { get; private set; } = default!;
    internal static PerishService FreshnessService { get; private set; } = default!;
    private string SettingsFile => $"{Mod.Info.ModID}.json";

    public OfflineFoodNoSpoil()
    {
        Instance = this;
    }

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide == EnumAppSide.Server;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        Server = api;
        // loading to trigger the file creation if it doesn't exist yet
        var settings = LoadSettings();
        ConditionalLogger.Settings = settings;

        ConditionalLogger.Info($"Starting {Mod.Info.Name}");

        FreshnessService = new PerishService(Server);
        Server.Event.PlayerJoin += Event_PlayerJoin;
    }

    private void Event_PlayerJoin(IServerPlayer byPlayer)
    {
        var settings = LoadSettings();

        ConditionalLogger.Settings = settings;

        ConditionalLogger.Debug($"Player {byPlayer.PlayerName} joined");

        if (settings.EnableMod)
        {
            foreach (var inventory in byPlayer.InventoryManager.Inventories.Values)
            {
                if (inventory is not null)
                {
                    FreshnessService.PreventInventorySpoil(inventory, settings);
                }
            }
        }
        else
        {
            ConditionalLogger.Debug($"Mod is disabled, aborting");
        }
    }


    private Settings LoadSettings()
    {
        try
        {
            var settings = Server.LoadModConfig<Settings>(SettingsFile);
            if (settings is null)
            {
                Server.StoreModConfig(Settings.Default, SettingsFile);
                return Settings.Default;
            }
            else
            {
                // force update to new version, so that it includes all new fields
                Server.StoreModConfig(settings, SettingsFile);
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
            if (hasErrors)
            {
                Server.StoreModConfig(settings, SettingsFile);
            }
            return settings;
        }
        catch
        {
            Server.StoreModConfig(Settings.Default, SettingsFile);
            return Settings.Default;
        }
    }
}