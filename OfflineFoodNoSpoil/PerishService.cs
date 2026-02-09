using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class PerishService
{
    private static readonly string[] InventoriesBlacklist =
    [
        "creative"
    ];
    private readonly ICoreServerAPI server;
    internal PerishService(ICoreServerAPI server)
    {
        this.server = server;
    }

    internal void SaveSnapshot(IInventory? inventory, Settings settings)
    {
        if (inventory is null)
        {
            return;
        }

        using (ConditionalLogger.Indent())
        {
            try
            {
                if (InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.Ordinal))
                {
                    ConditionalLogger.Debug($"Skipping inventory {inventory.ClassName} as it is not a supported inventory");
                    return;
                }
                foreach (var slot in inventory)
                {
                    if (slot?.Itemstack is not null)
                    {
                        try
                        {
                            var stack = slot.Itemstack;
                            ConditionalLogger.Debug($"Scanning slot {inventory.GetSlotId(slot)} {stack.GetName()}");
                            SnapPerish(new(stack), settings);
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

    internal void PreventInventorySpoil(IInventory? inventory, Settings settings)
    {
        if (inventory is null)
        {
            return;
        }

        using (ConditionalLogger.Indent())
        {
            try
            {
                if (InventoriesBlacklist.Contains(inventory.ClassName, StringComparer.Ordinal))
                {
                    ConditionalLogger.Debug($"Skipping inventory {inventory.ClassName} as it is not a supported inventory");
                    return;
                }
                foreach (var slot in inventory)
                {
                    if (slot?.Itemstack is not null)
                    {
                        try
                        {
                            var stack = slot.Itemstack;
                            ConditionalLogger.Debug($"Scanning slot {inventory.GetSlotId(slot)} {stack.GetName()}");
                            ResetPerish(new(stack), settings);
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
            item.UnFreezeTime(server.World, settings);
            item.SkipElapsedHours(server.World, settings);
            if (item.Contents is not null)
            {
                ConditionalLogger.Debug($"Scanning content of the stack");
                foreach (var content in item.Contents.Stacks)
                {
                    ResetPerish(new(content), settings);
                }
            }
        }
    }

    private void SnapPerish(ItemPerishValue item, Settings settings)
    {
        using (ConditionalLogger.Indent())
        {
            item.SnapState(server.World, settings);
            item.FreezeTime(server.World, settings);
            if (item.Contents is not null)
            {
                ConditionalLogger.Debug($"Scanning content of the stack");
                foreach (var content in item.Contents.Stacks)
                {
                    SnapPerish(new(content), settings);
                }
            }
        }
    }
}
