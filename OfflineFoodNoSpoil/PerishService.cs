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
        using (ConditionalLogger.Indent())
        {
            try
            {
                if (!InventoriesWhitelist.Contains(inventory.ClassName))
                {
                    ConditionalLogger.Debug($"Skipping inventory {inventory.ClassName} as it is not a supported inventory");
                    return;
                }
                foreach (var slot in inventory)
                {
                    if (slot.Itemstack is not null)
                    {
                        try
                        {
                            var stack = slot.Itemstack;
                            ConditionalLogger.Debug($"Scanning slot {inventory.GetSlotId(slot)} {stack.GetName()}");
                            var itemPerish = new ItemPerishValue(stack);
                            ResetPerish(itemPerish, settings);
                        }
                        catch (Exception e)
                        {
                            ConditionalLogger.Error(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConditionalLogger.Error(e);
            }
        }
    }

    private void ResetPerish(ItemPerishValue item, Settings settings)
    {
        using (ConditionalLogger.Indent())
        {
            item.ResetUpdatedTotalHours(server.World, settings);
            if (item.Content.Length > 0)
            {
                ConditionalLogger.Debug($"Scanning content of the stack");
                foreach (var content in item.Content)
                {
                    ResetPerish(content, settings);
                }
            }
        }
    }
}
