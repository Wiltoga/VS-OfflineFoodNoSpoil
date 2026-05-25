using NSubstitute;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil.Tests.Substitutes
{
    internal static class SubstituteInventory
    {
        public static InventoryBase Create() => Substitute.ForPartsOf<InventoryBase>("testinventory", Guid.NewGuid().ToString(), Scope.Inject<ICoreAPI>());
    }
}
