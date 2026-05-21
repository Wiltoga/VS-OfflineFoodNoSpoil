using AwesomeAssertions;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Wiltoga.OfflineFoodNoSpoil.Tests.Substitutes;

namespace Wiltoga.OfflineFoodNoSpoil.Tests;

public class ItemPerishServiceShould : ScopedTest
{
    private readonly ItemPerishService service;

    public ItemPerishServiceShould()
    {
        service = new();
    }

    [Fact]
    public void RetrieveAllPerishEntries()
    {
        Item perishable = new()
        {
            Code = new("test", "perishable"),
            TransitionableProps = [
                new()
                {
                    Type = EnumTransitionType.Perish,
                },
            ],
        };
        Item other = new()
        {
            Code = new("test", "other"),
        };

        ItemSlot slot = new(SubstituteInventory.Create())
        {
            Itemstack = new(other)
            {
                Attributes = new TreeAttribute()
                {
                    ["contents"] = new TreeAttribute()
                    {
                        ["1"] = new ItemstackAttribute()
                        {
                            value = new(perishable)
                            {
                                Attributes = new TreeAttribute()
                                {
                                    ["contents"] = new TreeAttribute()
                                    {
                                        ["aaa"] = new ItemstackAttribute()
                                        {
                                            value = new(perishable),
                                        },
                                    },
                                },
                            },
                        },
                        ["2"] = new ItemstackAttribute()
                        {
                            value = new(perishable),
                        },
                    },
                },
            },
        };

        var result = service.GetItemPerishEntries(slot);

        result.Should().HaveCount(4)
            .And.Satisfy(
                entry => entry.Name == $"{other.Code}" && entry.Key == "stack" && entry.TransitionState == null,
                entry => entry.Name == $"{other.Code}-{perishable.Code}[1]" && entry.Key == "stack:1" && entry.TransitionState != null,
                entry => entry.Name == $"{other.Code}-{perishable.Code}[2]" && entry.Key == "stack:2" && entry.TransitionState != null,
                entry => entry.Name == $"{other.Code}-{perishable.Code}[1]-{perishable.Code}[aaa]" && entry.Key == "stack:1:aaa" && entry.TransitionState != null
        );
    }
}
