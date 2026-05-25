using AwesomeAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Wiltoga.OfflineFoodNoSpoil.AttributeProxies;
using Wiltoga.OfflineFoodNoSpoil.Tests.Substitutes;

namespace Wiltoga.OfflineFoodNoSpoil.Tests;

public class ModDataManagerShould : ScopedTest
{
    private readonly ISaveGame saveGame;

    public ModDataManagerShould()
    {
        saveGame = Scope.Inject<ISaveGame>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("INVALID JSON")]
    [InlineData("[]")]
    [InlineData("null")]
    public void NotFailOnEmptyData(string? jsonData)
    {
        ItemSlot slot = new(SubstituteInventory.Create());
        saveGame.GetData<string>(ModDataManager.StorageKey).Returns(jsonData);

        using (ModDataManager service = new())
        {
            service.Invoking(s => s.TryGetModData(slot, [])).Should()
                .NotThrow();
        }
    }

    [Fact]
    public void RetrieveSavedModData()
    {
        ItemSlot slot = new(SubstituteInventory.Create());
        var jsonData = $$"""
            {
                "{{Key(slot)}}": {
                    "key1": {
                        "DisconnectTotalHours": 5
                    }
                }
            }
            """;
        saveGame.GetData<string>(ModDataManager.StorageKey).Returns(jsonData);

        Dictionary<string, ModData>? data;
        using (ModDataManager service = new())
        {
            data = service.TryGetModData(slot, []);
        }

        data.Should()
            .NotBeNull()
            .And.ContainKey("key1")
                .WhoseValue.Should().BeOfType<ModData>()
                .Which.DisconnectTotalHours.Should().Be(5);
        saveGame.Received(1).StoreData(ModDataManager.StorageKey, "{}");
    }

    [Fact]
    public void RetrieveOldModDataInItem()
    {
        ItemSlot slot = new(SubstituteInventory.Create());
        var oldModData = Substitute.For<ModDataProxy>([null]);
        oldModData.DisconnectTotalHours.Returns(5);
        oldModData.Exists.Returns(true);

        Dictionary<string, ModData>? data;
        using (ModDataManager service = new())
        {
            data = service.TryGetModData(slot, [
            new()
            {
                Contents = Substitute.For<ContentsProxy>([null]),
                TransitionState = Substitute.For<TransitionStateProxy>([null]),
                OldModData = oldModData,
                Key = "key1",
                Name = "",
            }]);
        }

        data.Should()
            .NotBeNull()
            .And.ContainKey("key1")
                .WhoseValue.Should().BeOfType<ModData>()
                .Which.DisconnectTotalHours.Should().Be(5);
        oldModData.Received(1).DeleteData();
    }

    [Fact]
    public void SaveModDataToGameFile()
    {
        var inventory = SubstituteInventory.Create();
        ItemSlot slot1 = new(inventory);
        ItemSlot slot2 = new(inventory);

        using (ModDataManager service = new())
        {
            service.SaveModData(slot1, new()
            {
                ["key1"] = new()
                {
                    DisconnectTotalHours = 1,
                }
            });
            service.SaveModData(slot2, new()
            {
                ["key2"] = new()
                {
                    DisconnectTotalHours = 2,
                }
            });
        }

        var serialized = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(
            $$"""
            {
                "{{Key(slot1)}}": {
                    "key1": {
                        "DisconnectTotalHours": 1
                    }
                },
                "{{Key(slot2)}}": {
                    "key2": {
                        "DisconnectTotalHours": 2
                    }
                }
            }
            """));
        saveGame.Received(1).StoreData(ModDataManager.StorageKey, serialized);
    }

    private static string Key(ItemSlot slot) => $"{slot.Inventory.InventoryID}[{slot.Inventory.GetSlotId(slot)}]";
}
