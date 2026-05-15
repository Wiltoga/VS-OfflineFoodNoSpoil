using System;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class PlayerEventsHandler : IPlayerEventsHandler
{
    private readonly IModLogger logger;
    private readonly IInventoryScanner inventoryScanner;
    private readonly Settings settings;

    public PlayerEventsHandler()
    {
        logger = Scope.Inject<IModLogger>();
        settings = Scope.Inject<ISettingsService>().Settings;
        inventoryScanner = Scope.Inject<IInventoryScanner>();
    }

    public void PlayerJoined(IServerPlayer byPlayer)
    {
        logger.Debug($"Player {byPlayer.PlayerName} joined");
        
        if (settings.EnableMod)
        {
            if (byPlayer.InventoryManager?.Inventories?.Values is null)
            {
                return;
            }

            using (logger.Indent())
            {
                try
                {

                    foreach (var inventory in byPlayer.InventoryManager.Inventories.Values)
                    {
                        if (inventory is not null && !settings.InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.OrdinalIgnoreCase))
                        {
                            inventoryScanner.FreezeInventory(inventory, byPlayer);
                        }
                        else
                        {
                            logger.Debug($"Incompatible inventory {inventory?.ClassName}");
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }
        else
        {
            logger.Debug($"Mod is disabled, aborting");
        }
    }

    public void PlayerDisconnected(IServerPlayer byPlayer)
    {
        logger.Debug($"Player {byPlayer.PlayerName} disconnected");

        if (settings.EnableMod)
        {
            try
            {
                if (byPlayer.InventoryManager?.Inventories?.Values is null)
                {
                    return;
                }
                using (logger.Indent())
                {
                    foreach (var inventory in byPlayer.InventoryManager.Inventories.Values)
                    {
                        if (inventory is not null && !settings.InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.OrdinalIgnoreCase))
                        {
                            inventoryScanner.UnfreezeInventory(inventory, byPlayer);
                        }
                        else
                        {
                            logger.Debug($"Incompatible inventory {inventory?.ClassName}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
        else
        {
            logger.Debug($"Mod is disabled, aborting");
        }
    }
}
