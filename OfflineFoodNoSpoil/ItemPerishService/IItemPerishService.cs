using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IItemPerishService
{
    ModData? FreezeItem(ItemPerishEntry item);

    ItemPerishEntry[] GetItemPerishEntries(ItemSlot slot);

    void UnfreezeItem(ItemPerishEntry item, ModData? modData);
}