using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Service to parse an inventory to snap or restore stuff
/// </summary>
public interface IInventorySnapper
{
    /// <summary>
    /// Setup a snapshot for the given inventory
    /// </summary>
    /// <param name="inventory"></param>
    void SnapInventory(IInventory inventory);

    /// <summary>
    /// Restore the spoil time of the inventory from when it got frozen
    /// </summary>
    /// <param name="inventory"></param>
    void RestoreInventory(IInventory inventory);
}