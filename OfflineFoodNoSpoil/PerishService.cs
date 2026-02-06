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
                            if (ResetPerish(new(stack), settings))
                            {
                                slot.MarkDirty();
                            }
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

    private bool ResetPerish(ItemPerishValue item, Settings settings)
    {
        using (ConditionalLogger.Indent())
        {
            var requiresUnFreeze = item.SkipElapsedHours(server.World, settings);
            if (requiresUnFreeze)
            {
                item.UnFreezeTime(server.World, settings);
            }
            if (item.Contents is not null)
            {
                ConditionalLogger.Debug($"Scanning content of the stack");
                foreach (var content in item.Contents.Stacks)
                {
                    requiresUnFreeze |= ResetPerish(new(content), settings);
                }
            }
            return requiresUnFreeze;
        }
    }

    private bool SnapPerish(ItemPerishValue item, Settings settings)
    {
        using (ConditionalLogger.Indent())
        {
            var requiresFreeze = item.SnapState(server.World, settings);
            if (requiresFreeze)
            {
                item.FreezeTime(server.World, settings);
            }
            if (item.Contents is not null)
            {
                ConditionalLogger.Debug($"Scanning content of the stack");
                foreach (var content in item.Contents.Stacks)
                {
                    requiresFreeze |= SnapPerish(new(content), settings);
                }
            }
            return requiresFreeze;
        }
    }
}
