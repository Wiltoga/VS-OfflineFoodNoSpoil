using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class InventoryScanner : IInventoryScanner
{
    private record SlotScan
    {
        public required ItemSlot Slot { get; init; }
        public required ItemPerishMapping[] Mappings { get; init; }
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

            logger.Debug($"Scanning slot {inventory.GetSlotId(slot)} {stack.GetName()}");

            var mappings = itemPerishService.GetItemPerishMappings(slot);

            yield return new()
            {
                Slot = slot,
                Mappings = mappings,
            };
        }
    }

    public void FreezeInventory(IInventory inventory, IPlayer player)
    {
        foreach(var scan in ScanInventory(inventory))
        {
            using (logger.Indent())
            {
                Dictionary<string, ModData> slotModData = [];
                foreach (var mapping in scan.Mappings)
                {
                    using (logger.Indent())
                    {
                        var modData = itemPerishService.FreezeItem(inventory, mapping);

                        if (modData is not null)
                        {
                            slotModData[mapping.Key] = modData;
                        }
                    }
                }
                if (slotModData.Keys.Count > 0)
                {
                    modDataManager.SaveModData(player, inventory, scan.Slot, slotModData);
                }
                else
                {
                    logger.Debug($"No data to save");
                }
            }
        }
    }

    public void UnfreezeInventory(IInventory inventory, IPlayer player)
    {
        foreach (var scan in ScanInventory(inventory))
        {
            using (logger.Indent())
            {
                var slotModData = modDataManager.TryGetModData(player, inventory, scan.Slot, scan.Mappings);

                foreach (var mapping in scan.Mappings)
                {
                    ModData? modData = null;
                    slotModData?.TryGetValue(mapping.Key, out modData);

                    itemPerishService.UnfreezeItem(inventory, mapping, modData);
                }
            }
        }
    }
}
