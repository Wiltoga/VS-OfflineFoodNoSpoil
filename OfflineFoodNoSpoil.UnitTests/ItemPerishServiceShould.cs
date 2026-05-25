using AwesomeAssertions;
using NSubstitute;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Wiltoga.OfflineFoodNoSpoil.AttributeProxies;
using Wiltoga.OfflineFoodNoSpoil.UnitTests.Substitutes;

namespace Wiltoga.OfflineFoodNoSpoil.UnitTests;

public class ItemPerishServiceShould : ScopedTest
{
    private readonly ItemPerishService service;
    private readonly IGameCalendar calendar;
    private readonly ITimeService timeService;

    public ItemPerishServiceShould()
    {
        service = new();
        calendar = Scope.Inject<IGameCalendar>();
        timeService = Scope.Inject<ITimeService>();
    }

    [Fact]
    public void FreezeItemWithTransitionState()
    {
        ItemPerishEntry entry = new()
        {
            TransitionState = Substitute.For<TransitionStateProxy>([null]),
            Contents = Substitute.For<ContentsProxy>([null]),
            Key = "key",
            Name = "name",
            OldModData = Substitute.For<ModDataProxy>([null]),
        };
        calendar.TotalHours.Returns(4.5);

        var saveData = service.FreezeItem(entry);

        saveData.Should()
            .NotBeNull()
            .And.BeOfType<ModData>()
            .Which.DisconnectTotalHours.Should().Be(4.5);
    }

    [Fact]
    public void UnfreezeItemWithModData()
    {
        ItemPerishEntry entry = new()
        {
            TransitionState = Substitute.For<TransitionStateProxy>([null]),
            Contents = Substitute.For<ContentsProxy>([null]),
            Key = "key",
            Name = "name",
            OldModData = Substitute.For<ModDataProxy>([null]),
        };
        entry.TransitionState.FreshHours.Returns([1, 2, 3]);
        entry.TransitionState.TransitionHours.Returns([10, 20, 30]);
        timeService.GetSkippedTimeSince(default!).Returns(5);
        ModData modData = new()
        {
            DisconnectTotalHours = 0,
        };
        entry.TransitionState.WhenForAnyArgs(p => p.FreshHours = default!).Do(call =>
        {
            var array = call.Arg<float[]>();
            array.Should().SatisfyRespectively(
            value => value.Should().Be(5 + 1),
            value => value.Should().Be(5 + 2),
            value => value.Should().Be(5 + 3)
                );
        });
        entry.TransitionState.WhenForAnyArgs(p => p.TransitionHours = default!).Do(call =>
        {
            var array = call.Arg<float[]>();
            array.Should().SatisfyRespectively(
            value => value.Should().Be(5 + 10),
            value => value.Should().Be(5 + 20),
            value => value.Should().Be(5 + 30)
                );
        });

        service.UnfreezeItem(entry, modData);

        entry.TransitionState.Should().Satisfy<TransitionStateProxy>(proxy =>
        {
            _ = proxy.ReceivedWithAnyArgs().FreshHours;
            _ = proxy.ReceivedWithAnyArgs().TransitionHours;
        });
    }

    [Fact]
    public void NotFreezeItemWithoutTransitionState()
    {
        ItemPerishEntry entry = new()
        {
            TransitionState = null,
            Contents = Substitute.For<ContentsProxy>([null]),
            Key = "key",
            Name = "name",
            OldModData = Substitute.For<ModDataProxy>([null]),
        };

        var saveData = service.FreezeItem(entry);

        saveData.Should()
            .BeNull();
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
