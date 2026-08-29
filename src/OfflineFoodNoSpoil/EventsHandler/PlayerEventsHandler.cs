using System;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Service used to start the snap and restore processes on player disconnect and join
/// </summary>
internal class PlayerEventsHandler : IPlayerEventsHandler
{
    private readonly IModLogger logger;
    private readonly IInventorySnapper inventoryScanner;
    private readonly ISettingsService settingsService;

    public PlayerEventsHandler()
    {
        logger = Scope.Inject<IModLogger>();
        settingsService = Scope.Inject<ISettingsService>();
        inventoryScanner = Scope.Inject<IInventorySnapper>();
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
                        logger.Debug($"Restore inventory {inventory?.ClassName}");
                        if (inventory is not null && !settingsService.Settings.InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.OrdinalIgnoreCase))
                        {
                            using (logger.Indent())
                            {
                                inventoryScanner.RestoreInventory(inventory);
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
                        logger.Debug($"Save inventory {inventory?.ClassName}");
                        if (inventory is not null && !settingsService.Settings.InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.OrdinalIgnoreCase))
                        {
                            using (logger.Indent())
                            {
                                inventoryScanner.SnapInventory(inventory);
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
