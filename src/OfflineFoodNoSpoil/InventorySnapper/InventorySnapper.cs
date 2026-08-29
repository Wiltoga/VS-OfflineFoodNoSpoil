using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class InventorySnapper : IInventorySnapper
{
    private record SlotScan
    {
        public required ItemSlot Slot { get; init; }
        public required ItemPerishEntry[] Entries { get; init; }
    }

    private readonly IModLogger logger;
    private readonly IItemPerishService itemPerishService;
    private readonly IModDataManager modDataManager;

    public InventorySnapper()
    {
        logger = Scope.Inject<IModLogger>();
        itemPerishService = Scope.Inject<IItemPerishService>();
        modDataManager = Scope.Inject<IModDataManager>();
    }

    /// <summary>
    /// Scans all slots of the inventory and returns the ones with perishable items
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    private IEnumerable<SlotScan> ScanInventory(IInventory inventory)
    {
        foreach (var slot in inventory.Where(slot => slot?.Itemstack is not null))
        {
            ItemPerishEntry[]? entries = null;
            try
            {
                var stack = slot.Itemstack!;

                logger.Debug($"Scanning slot {inventory.GetSlotId(slot)} {stack.Collectible?.Code}");

                entries = itemPerishService.GetItemPerishEntries(slot);

            }
            catch (Exception e)
            {
                LogErroredSlot(slot, e);
            }
            if (entries is not null)
            {
                yield return new()
                {
                    Slot = slot,
                    Entries = entries,
                };
            }
        }
    }

    public void SnapInventory(IInventory inventory)
    {
        foreach(var scan in ScanInventory(inventory))
        {
            try
            {
                using (logger.Indent())
                {
                    // save data for one inventory slot
                    Dictionary<string, ModData> slotModData = [];
                    foreach (var entry in scan.Entries)
                    {
                        using (logger.Indent())
                        {
                            var modData = itemPerishService.SnapItem(entry);

                            if (modData is not null)
                            {
                                slotModData[entry.Key] = modData;
                            }
                        }
                    }
                    if (slotModData.Keys.Count > 0)
                    {
                        modDataManager.SaveModData(scan.Slot, slotModData);
                    }
                    else
                    {
                        logger.Debug($"No data to save");
                    }
                }
            }
            catch (Exception e)
            {
                LogErroredSlot(scan.Slot, e);
            }
        }
    }

    public void RestoreInventory(IInventory inventory)
    {
        foreach (var scan in ScanInventory(inventory))
        {
            try
            {
                using (logger.Indent())
                {
                    var slotModData = modDataManager.TryGetModData(scan.Slot, scan.Entries);

                    foreach (var entry in scan.Entries)
                    {
                        ModData? modData = null;
                        slotModData?.TryGetValue(entry.Key, out modData);

                        itemPerishService.RestoreItem(entry, modData);
                    }
                }
            }
            catch (Exception e)
            {
                LogErroredSlot(scan.Slot, e);
            }
        }
    }

    /// <summary>
    /// Logs infos about an item slot and create a crash dump for debugging purposes
    /// </summary>
    /// <param name="itemSlot"></param>
    /// <param name="e"></param>
    private void LogErroredSlot(ItemSlot itemSlot, Exception e)
    {
        StringBuilder builder = new();
        builder.AppendLine($"Code: {itemSlot.Itemstack?.Collectible?.Code}");
        if (itemSlot.Itemstack is not null)
        {
            builder.AppendLine();
            builder.AppendLine(Convert.ToBase64String(itemSlot.Itemstack.ToBytes()));
        }
        builder.AppendLine();
        builder.AppendLine(e.ToString());
        var file = logger.CreateCrashDump(builder.ToString());
        if (file is not null)
        {
            logger.Warning($"SOMETHING WENT WRONG DURING FOOD HANDLING.\nIt would be kind to provide me the crash dump here : 'file:///{file.Replace('\\', '/')}' on github (https://github.com/Wiltoga/VS-OfflineFoodNoSpoil/issues), or even to me by Discord (@wiltoga).\nIt only consists of the item code and its attributes, no personal data.");
        }
        logger.Error(e);
    }
}
