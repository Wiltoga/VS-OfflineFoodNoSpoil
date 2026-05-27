using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IModDataManager
{
    // TODO: delete the entrys parameter when full 2.0 of the mod comes out
    Dictionary<string, ModData>? TryGetModData(ItemSlot slot, IEnumerable<ItemPerishEntry> entrys);
    void SaveModData(ItemSlot slot, Dictionary<string, ModData> data);
}
