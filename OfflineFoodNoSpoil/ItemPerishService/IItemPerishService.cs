using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal interface IItemPerishService
{
    void FreezeItem(IInventory inventory, ItemPerishMapping item);

    ItemPerishMapping[] GetItemPerishMappings(ItemSlot slot);

    void UnfreezeItem(IInventory inventory, ItemPerishMapping item);
}