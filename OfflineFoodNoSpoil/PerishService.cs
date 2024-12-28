using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class PerishService
{
    private static readonly string[] InventoriesWhitelist = new[]
 {
        "hotbar",
        "backpack",
    };
    private readonly ICoreServerAPI server;
    internal PerishService(ICoreServerAPI server)
    {
        this.server = server;
    }

    internal void PreventInventorySpoil(IInventory inventory, Settings settings)
    {
        try
        {
            if (settings.UseLogs)
                server.Logger.VerboseDebug($"Scanning inventory {inventory.ClassName} {inventory.InventoryID}");
            if (!InventoriesWhitelist.Contains(inventory.ClassName))
            {
                server.Logger.VerboseDebug($"Skipping inventory {inventory.ClassName} {inventory.InventoryID}");
                return;
            }
            foreach (var slot in inventory)
            {
                if (slot.Itemstack is not null)
                {
                    try
                    {
                        var stack = slot.Itemstack;
                        if (settings.UseLogs)
                            server.Logger.VerboseDebug($"Scanning slot {inventory.GetSlotId(slot)} {stack.GetName()}");
                        var itemPerish = new ItemPerishValue(stack);
                        ResetPerish(itemPerish, settings);
                    }
                    catch (Exception e)
                    {
                        if (settings.UseLogs)
                            server.Logger.Error(e);
                    }
                }
            }
        }
        catch (Exception e)
        {
            if (settings.UseLogs)
                server.Logger.Error(e);
        }
    }

    private void ResetPerish(ItemPerishValue item, Settings settings)
    {
        item.ResetUpdatedTotalHours(server.World, settings);
        foreach (var content in item.Content)
        {
            ResetPerish(content, settings);
        }
    }
}
