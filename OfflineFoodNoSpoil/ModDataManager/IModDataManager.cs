using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IModDataManager
{
    // TODO: delete the entrys parameter when full 2.0 of the mod comes out
    Dictionary<string, ModData>? TryGetModData(IInventory inventory, ItemSlot slot, IEnumerable<ItemPerishEntry> entrys);
    void SaveModData(IInventory inventory, ItemSlot slot, Dictionary<string, ModData> data);
}
