using AwesomeAssertions;
using NSubstitute;
using Vintagestory.API.Common;
using Wiltoga.OfflineFoodNoSpoil.Tests.Substitutes;

namespace Wiltoga.OfflineFoodNoSpoil.Tests;

public class InventoryScannerShould : ScopedTest
{
    private readonly InventoryScanner scanner;
    private readonly IItemPerishService itemPerishService;
    private readonly IModDataManager modDataManager;

    public InventoryScannerShould()
    {
        scanner = new();
        itemPerishService = Scope.Inject<IItemPerishService>();
        modDataManager = Scope.Inject<IModDataManager>();
    }

    [Fact]
    public void FreezePerishEntries()
    {
        var inventory = SubstituteInventory.Create();
        ItemSlot slot = new(inventory)
        {
            Itemstack = new(),
        };
        IEnumerator<ItemSlot> items()
        {
            yield return slot;
        }
        inventory.GetEnumerator().Returns(_ => items());
        ItemPerishEntry[] entries;
        itemPerishService.GetItemPerishEntries(slot).Returns(entries =
        [
            new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = string.Empty,
                Name = string.Empty,
            },
            new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = string.Empty,
                Name = string.Empty,
            },
        ]);

        scanner.FreezeInventory(inventory);

        itemPerishService.Received(1).FreezeItem(entries[0]);
        itemPerishService.Received(1).FreezeItem(entries[1]);
    }

    [Fact]
    public void UnfreezePerishEntries()
    {
        var inventory = SubstituteInventory.Create();
        ItemSlot slot = new(inventory)
        {
            Itemstack = new(),
        };
        IEnumerator<ItemSlot> items()
        {
            yield return slot;
        }
        inventory.GetEnumerator().Returns(_ => items());
        ItemPerishEntry[] entries;
        itemPerishService.GetItemPerishEntries(slot).Returns(entries =
        [
            new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = string.Empty,
                Name = string.Empty,
            },
            new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = string.Empty,
                Name = string.Empty,
            },
        ]);

        scanner.UnfreezeInventory(inventory);

        itemPerishService.Received(1).UnfreezeItem(entries[0], null);
        itemPerishService.Received(1).UnfreezeItem(entries[1], null);
    }

    [Fact]
    public void SaveModDataDuringFreeze()
    {
        var inventory = SubstituteInventory.Create();
        ItemSlot slot = new(inventory)
        {
            Itemstack = new(),
        };
        IEnumerator<ItemSlot> items()
        {
            yield return slot;
        }
        inventory.GetEnumerator().Returns(_ => items());
        ItemPerishEntry entry1;
        ItemPerishEntry entry2;
        itemPerishService.GetItemPerishEntries(slot).Returns(
        [
            entry1 = new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = "entryKey1",
                Name = string.Empty,
            },
            entry2 = new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = "entryKey2",
                Name = string.Empty,
            },
        ]);
        ModData modData1;
        ModData modData2;
        itemPerishService.FreezeItem(entry1).Returns(modData1 = new()
        {
            DisconnectTotalHours = 10,
        });
        itemPerishService.FreezeItem(entry2).Returns(modData2 = new()
        {
            DisconnectTotalHours = 20,
        });
        modDataManager.When(manager => manager.SaveModData(slot, Arg.Any<Dictionary<string, ModData>>())).Do(call =>
        {
            var data = call.Arg<Dictionary<string, ModData>>();

            data.Should().ContainKey("entryKey1")
                .WhoseValue.Should().Be(modData1);

            data.Should().ContainKey("entryKey2")
                .WhoseValue.Should().Be(modData2);
        });

        scanner.FreezeInventory(inventory);

        modDataManager.Received(1).SaveModData(slot, Arg.Any<Dictionary<string, ModData>>());
    }

    [Fact]
    public void FetchModDataDuringUnfreeze()
    {
        var inventory = SubstituteInventory.Create();
        ItemSlot slot = new(inventory)
        {
            Itemstack = new(),
        };
        IEnumerator<ItemSlot> items()
        {
            yield return slot;
        }
        inventory.GetEnumerator().Returns(_ => items());
        ItemPerishEntry[] entries;
        itemPerishService.GetItemPerishEntries(slot).Returns(entries =
        [
            new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = "entryKey1",
                Name = string.Empty,
            },
            new()
            {
                Contents = new(null),
                OldModData = new(null),
                TransitionState = null,
                Key = "entryKey2",
                Name = string.Empty,
            },
        ]);
        ModData modData1 = new()
        {
            DisconnectTotalHours = 10,
        };
        ModData modData2 = new()
        {
            DisconnectTotalHours = 20,
        };
        modDataManager.TryGetModData(slot, entries).Returns(new Dictionary<string, ModData>
        {
            ["entryKey1"] = modData1,
            ["entryKey2"] = modData2,
        });

        scanner.UnfreezeInventory(inventory);

        modDataManager.Received(1).TryGetModData(slot, entries);
        itemPerishService.Received(1).UnfreezeItem(entries[0], modData1);
        itemPerishService.Received(1).UnfreezeItem(entries[1], modData2);
    }

    [Fact]
    public void NotFreezeSlotsWithNoStack()
    {
        var inventory = SubstituteInventory.Create();
        ItemSlot slot = new(inventory)
        {
            Itemstack = null,
        };
        IEnumerator<ItemSlot> items()
        {
            yield return slot;
        }
        inventory.GetEnumerator().Returns(_ => items());

        scanner.FreezeInventory(inventory);

        itemPerishService.DidNotReceive().GetItemPerishEntries(slot);
    }

    [Fact]
    public void NotUnfreezeSlotsWithNoStack()
    {
        var inventory = SubstituteInventory.Create();
        ItemSlot slot = new(inventory)
        {
            Itemstack = null,
        };
        IEnumerator<ItemSlot> items()
        {
            yield return slot;
        }
        inventory.GetEnumerator().Returns(_ => items());

        scanner.UnfreezeInventory(inventory);

        itemPerishService.DidNotReceive().GetItemPerishEntries(slot);
    }
}
