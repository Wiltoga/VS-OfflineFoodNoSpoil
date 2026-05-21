using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IItemPerishService
{
    ModData? FreezeItem(IInventory inventory, ItemPerishEntry item);

    ItemPerishEntry[] GetItemPerishEntries(ItemSlot slot);

    void UnfreezeItem(IInventory inventory, ItemPerishEntry item, ModData? modData);
}