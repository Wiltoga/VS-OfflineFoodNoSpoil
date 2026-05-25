using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class ItemPerishService : IItemPerishService
{
    private readonly IModLogger logger;
    private readonly ITimeService timeSkipService;
    private readonly IGameCalendar calendar;

    public ItemPerishService()
    {
        logger = Scope.Inject<IModLogger>();
        timeSkipService = Scope.Inject<ITimeService>();
        calendar = Scope.Inject<IGameCalendar>();
    }

    public ModData? FreezeItem(ItemPerishEntry item)
    {
        logger.Debug($"Freeze item {item.Name}");
        if (item.TransitionState is not null)
        {
            logger.Debug($"Item has transition state");
            return new()
            {
                DisconnectTotalHours = calendar.TotalHours,
            };
        }
        else
        {
            logger.Debug($"Item has no transition state");
            return null;
        }
    }

    public void UnfreezeItem(ItemPerishEntry item, ModData? modData)
    {
        logger.Debug($"Unfreeze item {item.Name}");
        if (modData is null)
        {
            logger.Debug($"No mod data provided");
            return;
        }
        if (item.TransitionState is not null)
        {
            logger.Debug($"Item has transition state");
            if (item.TransitionState.FreshHours is null)
            {
                logger.Warning($"Invalid item {item.Name} : no FreshHours in attributes");
                return;
            }
            if (item.TransitionState.TransitionHours is null)
            {
                logger.Warning($"Invalid item {item.Name} : no TransitionHours in attributes");
                return;
            }
            var skippedTime = timeSkipService.GetSkippedTimeSince(modData.DisconnectTotalHours);

            item.TransitionState.FreshHours = item.TransitionState.FreshHours.Select(hours => hours + skippedTime).ToArray();
            item.TransitionState.TransitionHours = item.TransitionState.TransitionHours.Select(hours => hours + skippedTime).ToArray();
        }
        else
        {
            logger.Debug($"Item has no transition state");
            return;
        }
    }

    public ItemPerishEntry[] GetItemPerishEntries(ItemSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot.Itemstack);
        var stack = slot.Itemstack;

        return GetAllEntries(slot.Itemstack, null, default).ToArray();
    }

    private IEnumerable<ItemPerishEntry> GetAllEntries(ItemStack stack, ItemPerishEntry? parent, string? index)
    {
        using (logger.Indent())
        {
            var current = GetEntry(stack, parent, index);

            yield return current;

            if (current.Contents.Stacks.Keys.Count == 0)
            {
                logger.Debug($"Stack {stack.Collectible?.Code} has no content");
            }

            foreach(var pair in current.Contents.Stacks)
            {
                var subStack = pair.Value;
                foreach (var subEntry in GetAllEntries(subStack, current, pair.Key))
                {
                    yield return subEntry;
                }
            }
        }
    }

    private ItemPerishEntry GetEntry(ItemStack stack, ItemPerishEntry? parent, string? index)
    {
        var (contents, transitionState, oldModData) = GetProxies(stack);

        return new()
        {
            Name = parent is null ? $"{stack.Collectible?.Code}" : $"{parent.Name}-{stack.Collectible?.Code}[{index}]",
            Key = parent is null ? "stack" : $"{parent.Key}:{index}",
            Contents = contents,
            TransitionState = transitionState,
            OldModData = oldModData,
        };
    }

    private (ContentsProxy, TransitionStateProxy?, ModDataProxy) GetProxies(ItemStack stack)
    {
        TransitionStateProxy? transitionstate = null;
        ContentsProxy contents = new(stack.Attributes);
        ModDataProxy oldModData = new(stack.Attributes);

        if (stack.Collectible?.TransitionableProps?.Any(property => property?.Type is EnumTransitionType.Perish) is not null)
        {
            logger.Debug($"Stack {stack.Collectible?.Code} is perishable");
            transitionstate = new(stack.Attributes);
        }
        else
        {
            logger.Debug($"Stack {stack.Collectible?.Code} is not perishable");
        }

        return (contents, transitionstate, oldModData);
    }
}
