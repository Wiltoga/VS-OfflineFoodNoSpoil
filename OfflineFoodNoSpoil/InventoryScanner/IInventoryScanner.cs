using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IInventoryScanner
{
    void FreezeInventory(IInventory inventory, IPlayer player);

    void UnfreezeInventory(IInventory inventory, IPlayer player);
}