using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IInventoryScanner
{
    void FreezeInventory(IInventory inventory);

    void UnfreezeInventory(IInventory inventory);
}