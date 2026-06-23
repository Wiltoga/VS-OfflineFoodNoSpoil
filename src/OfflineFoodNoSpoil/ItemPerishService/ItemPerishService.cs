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

    /// <summary>
    /// The name of the key to index the perish data of the slot itself.
    /// </summary>
    private const string ItemSlotRootKey = "stack";

    /// <summary>
    /// The name of the key to index the perish data of one of the contents of a stack.
    /// </summary>
    /// First parameter is the parent key, second is the index
    private const string ContentsKeyFormat = "{0}:{1}";

    public ItemPerishService()
    {
        logger = Scope.Inject<IModLogger>();
        timeSkipService = Scope.Inject<ITimeService>();
        calendar = Scope.Inject<IGameCalendar>();
    }

    public ModData? SnapItem(ItemPerishEntry item)
    {
        logger.Debug($"Save item {item.Name}");
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

    public void RestoreItem(ItemPerishEntry item, ModData? modData)
    {
        logger.Debug($"Restore item {item.Name}");
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

            // add to the freshhours the computed time to skip
            // let's hope this way works all the time
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

    /// <summary>
    /// Recursive method to extract all perishable entries of a stack
    /// </summary>
    /// <param name="stack">The stack to parse its attributes</param>
    /// <param name="parent">The parent entry, or <c>null</c> if the stack is the inventory slot</param>
    /// <param name="index">The name of the key index in case the entry is a child of a contents attribute</param>
    /// <returns></returns>
    private IEnumerable<ItemPerishEntry> GetAllEntries(ItemStack stack, ItemPerishEntry? parent, string? index)
    {
        using (logger.Indent())
        {
            // first retrieve the data of the stack itself
            var current = GetEntry(stack, parent, index);

            yield return current;

            if (current.Contents.Stacks.Keys.Count == 0)
            {
                logger.Debug($"Stack {stack.Collectible?.Code} has no content");
            }

            foreach(var pair in current.Contents.Stacks)
            {
                var subStack = pair.Value;
                // then call the same method again to parse all its contents (if any)
                foreach (var subEntry in GetAllEntries(subStack, current, pair.Key))
                {
                    yield return subEntry;
                }
            }
        }
    }

    private ItemPerishEntry GetEntry(ItemStack stack, ItemPerishEntry? parent, string? index)
    {
        // create all proxies to the attributes, then create the entry
        var (contents, transitionState, oldModData) = GetProxies(stack);

        return new()
        {
            Name = parent is null ? $"{stack.Collectible?.Code}" : $"{parent.Name}-{stack.Collectible?.Code}[{index}]",
            Key = parent is null ? ItemSlotRootKey : string.Format(ContentsKeyFormat, parent.Key, index),
            Contents = contents,
            TransitionState = transitionState,
            OldModData = oldModData,
        };
    }

    /// <summary>
    /// Creates all attribute proxies to create the perish entry
    /// </summary>
    /// <param name="stack"></param>
    /// <returns></returns>
    private (ContentsProxy, TransitionStateProxy?, ModDataProxy) GetProxies(ItemStack stack)
    {
        TransitionStateProxy? transitionstate = null;
        ContentsProxy contents = new(stack.Attributes);
        ModDataProxy oldModData = new(stack.Attributes);

        // only create the transitionstate proxy if the item is actually spoilable,
        // to avoid skipping a dryable item or any other timed conversion
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
