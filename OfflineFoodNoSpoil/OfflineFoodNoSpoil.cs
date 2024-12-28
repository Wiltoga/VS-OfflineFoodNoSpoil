using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

public class OfflineFoodNoSpoil : ModSystem
{
    public static ICoreServerAPI Server { get; private set; } = default!;
    internal static PerishService FreshnessService { get; private set; } = default!;
    private string SettingsFile => $"{Mod.Info.ModID}.json";

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide == EnumAppSide.Server;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        Server = api;
        FreshnessService = new PerishService(Server);
        // loading to trigger the file creation if it doesn't exist yet
        LoadSettings();
        Server.Event.PlayerJoin += Event_PlayerJoin;
    }

    private void Event_PlayerJoin(IServerPlayer byPlayer)
    {
        var settings = LoadSettings();

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
            if (settings.FoodSpoilMultiplier < 0 || settings.FoodSpoilMultiplier > 1)
            {
                settings.FoodSpoilMultiplier = Math.Clamp(settings.FoodSpoilMultiplier, 0, 1);
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