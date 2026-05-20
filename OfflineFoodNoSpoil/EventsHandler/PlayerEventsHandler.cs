using System;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class PlayerEventsHandler : IPlayerEventsHandler
{
    private readonly IModLogger logger;
    private readonly IInventoryScanner inventoryScanner;
    private readonly ISettingsService settingsService;

    public PlayerEventsHandler()
    {
        logger = Scope.Inject<IModLogger>();
        settingsService = Scope.Inject<ISettingsService>();
        inventoryScanner = Scope.Inject<IInventoryScanner>();
    }

    public void PlayerJoined(IServerPlayer byPlayer)
    {
        logger.Debug($"Player {byPlayer.PlayerName} joined");
        
        if (settingsService.Settings.EnableMod)
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
                        logger.Debug($"Unfreeze inventory {inventory?.ClassName}");
                        if (inventory is not null && !settingsService.Settings.InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.OrdinalIgnoreCase))
                        {
                            using (logger.Indent())
                            {
                                inventoryScanner.UnfreezeInventory(inventory, byPlayer);
                            }
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

        if (settingsService.Settings.EnableMod)
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
                        logger.Debug($"Freeze inventory {inventory?.ClassName}");
                        if (inventory is not null && !settingsService.Settings.InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.OrdinalIgnoreCase))
                        {
                            using (logger.Indent())
                            {
                                inventoryScanner.FreezeInventory(inventory, byPlayer);
                            }
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
