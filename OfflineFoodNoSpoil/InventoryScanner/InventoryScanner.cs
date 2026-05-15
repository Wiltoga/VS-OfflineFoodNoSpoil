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
            var modData = modDataManager.TryGetModData(player, inventory, scan.Slot, scan.Mappings);

        }
    }

    public void UnfreezeInventory(IInventory inventory, IPlayer player)
    {
        ScanInventory(inventory);
    }
}
