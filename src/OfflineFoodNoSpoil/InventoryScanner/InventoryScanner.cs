using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class InventoryScanner : IInventoryScanner
{
    private record SlotScan
    {
        public required ItemSlot Slot { get; init; }
        public required ItemPerishEntry[] Entries { get; init; }
    }

    private readonly IModLogger logger;
    private readonly IItemPerishService itemPerishService;
    private readonly IModDataManager modDataManager;

    public InventoryScanner()
    {
        logger = Scope.Inject<IModLogger>();
        itemPerishService = Scope.Inject<IItemPerishService>();
        modDataManager = Scope.Inject<IModDataManager>();
    }

    private IEnumerable<SlotScan> ScanInventory(IInventory inventory)
    {
        foreach (var slot in inventory.Where(slot => slot?.Itemstack is not null))
        {
            var stack = slot.Itemstack!;

            logger.Debug($"Scanning slot {inventory.GetSlotId(slot)} {stack.Collectible?.Code}");

            var entries = itemPerishService.GetItemPerishEntries(slot);

            yield return new()
            {
                Slot = slot,
                Entries = entries,
            };
        }
    }

    public void FreezeInventory(IInventory inventory)
    {
        foreach(var scan in ScanInventory(inventory))
        {
            using (logger.Indent())
            {
                Dictionary<string, ModData> slotModData = [];
                foreach (var entry in scan.Entries)
                {
                    using (logger.Indent())
                    {
                        var modData = itemPerishService.FreezeItem(entry);

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
    }

    public void UnfreezeInventory(IInventory inventory)
    {
        foreach (var scan in ScanInventory(inventory))
        {
            using (logger.Indent())
            {
                var slotModData = modDataManager.TryGetModData(scan.Slot, scan.Entries);

                foreach (var entry in scan.Entries)
                {
                    ModData? modData = null;
                    slotModData?.TryGetValue(entry.Key, out modData);

                    itemPerishService.UnfreezeItem(entry, modData);
                }
            }
        }
    }
}
