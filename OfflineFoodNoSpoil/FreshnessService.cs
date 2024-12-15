using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class FreshnessService
{
    private static readonly string[] InventoriesWhitelist = new[]
 {
        "hotbar",
        "backpack",
    };
    private readonly ICoreServerAPI server;
    internal FreshnessService(ICoreServerAPI server)
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
                        var itemFreshness = new ItemFreshness(stack);
                        if (itemFreshness.HasFreshness)
                        {
                            foreach (var state in itemFreshness.AllStates)
                            {
                                var skip = (float)(server.World.Calendar.TotalHours - state.LastUpdatedTotalHours.value);
                                skip *= 1 - settings.FoodSpoilMultiplier;
                                if (settings.UseLogs)
                                    server.Logger.VerboseDebug("Skip time : " + skip);
                                state.LastUpdatedTotalHours.value += skip;
                            }
                            slot.MarkDirty();
                        }
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
}
