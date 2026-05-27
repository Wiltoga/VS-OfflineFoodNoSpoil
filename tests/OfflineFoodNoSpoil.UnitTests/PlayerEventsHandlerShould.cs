using NSubstitute;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil.UnitTests;

public class PlayerEventsHandlerShould : ScopedTest
{
    private readonly PlayerEventsHandler handler;

    public PlayerEventsHandlerShould()
    {
        handler = new();
    }

    [Fact]
    public void NotRunWhenModDisabled()
    {
        var inventoryScanner = Scope.Inject<IInventoryScanner>();
        Scope.Inject<ISettingsService>().Settings.Returns(Settings.Default with
        {
            EnableMod = false,
        });
        var player = Substitute.For<IServerPlayer>();

        handler.PlayerJoined(player);
        handler.PlayerDisconnected(player);

        inventoryScanner.DidNotReceiveWithAnyArgs().FreezeInventory(default!);
        inventoryScanner.DidNotReceiveWithAnyArgs().UnfreezeInventory(default!);
    }

    [Fact]
    public void UnfreezeInventoryOnJoin()
    {
        var inventoryScanner = Scope.Inject<IInventoryScanner>();
        var player = Substitute.For<IServerPlayer>();
        player.InventoryManager.Returns(Substitute.For<IPlayerInventoryManager>());
        player.InventoryManager.Inventories.Returns(new Dictionary<string, IInventory>
        {
            ["1"] = Substitute.For<IInventory>(),
        });
        var inventory = player.InventoryManager.Inventories["1"];

        handler.PlayerJoined(player);

        inventoryScanner.Received(1).UnfreezeInventory(inventory);
    }

    [Fact]
    public void FreezeInventoryOnDisconnect()
    {
        var inventoryScanner = Scope.Inject<IInventoryScanner>();
        var player = Substitute.For<IServerPlayer>();
        player.InventoryManager.Returns(Substitute.For<IPlayerInventoryManager>());
        player.InventoryManager.Inventories.Returns(new Dictionary<string, IInventory>
        {
            ["1"] = Substitute.For<IInventory>(),
        });
        var inventory = player.InventoryManager.Inventories["1"];

        handler.PlayerDisconnected(player);

        inventoryScanner.Received(1).FreezeInventory(inventory);
    }

    [Fact]
    public void NotUnfreezeBlacklistedInventoryOnJoin()
    {
        Scope.Inject<ISettingsService>().Settings.Returns(Settings.Default with
        {
            InventoriesBlacklist = ["forbidden"],
        });
        var inventoryScanner = Scope.Inject<IInventoryScanner>();
        var player = Substitute.For<IServerPlayer>();
        player.InventoryManager.Returns(Substitute.For<IPlayerInventoryManager>());
        player.InventoryManager.Inventories.Returns(new Dictionary<string, IInventory>
        {
            ["1"] = Substitute.For<IInventory>(),
        });
        var inventory = player.InventoryManager.Inventories["1"];
        inventory.ClassName.Returns("forbidden");

        handler.PlayerJoined(player);

        inventoryScanner.DidNotReceive().UnfreezeInventory(inventory);
    }

    [Fact]
    public void NotFreezeBlacklistedInventoryOnDisconnect()
    {
        Scope.Inject<ISettingsService>().Settings.Returns(Settings.Default with
        {
            InventoriesBlacklist = ["forbidden"],
        });
        var inventoryScanner = Scope.Inject<IInventoryScanner>();
        var player = Substitute.For<IServerPlayer>();
        player.InventoryManager.Returns(Substitute.For<IPlayerInventoryManager>());
        player.InventoryManager.Inventories.Returns(new Dictionary<string, IInventory>
        {
            ["1"] = Substitute.For<IInventory>(),
        });
        var inventory = player.InventoryManager.Inventories["1"];
        inventory.ClassName.Returns("forbidden");

        handler.PlayerDisconnected(player);

        inventoryScanner.DidNotReceive().FreezeInventory(inventory);
    }
}
