using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Service to extract perishable data from a slot and snap/restore it
/// </summary>
public interface IItemPerishService
{
    /// <summary>
    /// Creates a snapshot of the perishable entry, used to restore it later
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ModData? SnapItem(ItemPerishEntry item);

    /// <summary>
    /// Extracts all perishable entries of a slot, if any
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    ItemPerishEntry[] GetItemPerishEntries(ItemSlot slot);

    /// <summary>
    /// Restores the given entry to a previously snapped state
    /// </summary>
    /// <param name="item"></param>
    /// <param name="modData"></param>
    void RestoreItem(ItemPerishEntry item, ModData? modData);
}