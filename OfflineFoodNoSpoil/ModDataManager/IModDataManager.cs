using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IModDataManager
{
    // TODO: delete the mappings parameter when full 2.0 of the mod comes out
    Dictionary<string, ModData>? TryGetModData(IPlayer player, IInventory inventory, ItemSlot slot, ItemPerishMapping[] mappings);
    void SaveModData(IPlayer player, IInventory inventory, ItemSlot slot, Dictionary<string, ModData> data);
}
