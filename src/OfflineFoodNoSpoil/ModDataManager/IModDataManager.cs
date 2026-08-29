using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Service to save and retrieve mod data
/// </summary>
public interface IModDataManager
{
    // TODO: delete the entries parameter when full 2.0 of the mod comes out
    /// <summary>
    /// Retrieve mod data from a slot
    /// </summary>
    /// <param name="slot">Slot used as a reference to the data</param>
    /// <param name="entries">Perish entries for old mod data. Will be removed at some point</param>
    /// <returns>The saved data if any is found</returns>
    Dictionary<string, ModData>? TryGetModData(ItemSlot slot, IEnumerable<ItemPerishEntry> entries);

    /// <summary>
    /// Save data of an inventory slot
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="data"></param>
    void SaveModData(ItemSlot slot, Dictionary<string, ModData> data);
}
