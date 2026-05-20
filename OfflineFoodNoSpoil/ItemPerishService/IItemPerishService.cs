using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal interface IItemPerishService
{
    ModData? FreezeItem(IInventory inventory, ItemPerishMapping item);

    ItemPerishMapping[] GetItemPerishMappings(ItemSlot slot);

    void UnfreezeItem(IInventory inventory, ItemPerishMapping item, ModData? modData);
}