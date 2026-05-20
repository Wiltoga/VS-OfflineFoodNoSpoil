using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class ItemPerishService : IItemPerishService
{
    private readonly IModLogger logger;
    private readonly ITimeSkipService timeSkipService;
    private readonly ICoreServerAPI server;

    public ItemPerishService()
    {
        logger = Scope.Inject<IModLogger>();
        timeSkipService = Scope.Inject<ITimeSkipService>();
        server = Scope.Inject<ICoreServerAPI>();
    }

    public ModData? FreezeItem(IInventory inventory, ItemPerishMapping item)
    {
        logger.Debug($"Freeze item {item.Name}");
        if (item.TransitionState is not null)
        {
            logger.Debug($"Item has transition state");
            return new()
            {
                DisconnectTotalHours = server.World.Calendar.TotalHours,
            };
        }
        else
        {
            logger.Debug($"Item has no transition state");
            return null;
        }
    }

    public void UnfreezeItem(IInventory inventory, ItemPerishMapping item, ModData? modData)
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
            var skippedTime = timeSkipService.GetSkippedTime(modData.DisconnectTotalHours);

            item.TransitionState.FreshHours = item.TransitionState.FreshHours.Select(hours => hours + skippedTime).ToArray();
            item.TransitionState.TransitionHours = item.TransitionState.TransitionHours.Select(hours => hours + skippedTime).ToArray();
        }
        else
        {
            logger.Debug($"Item has no transition state");
            return;
        }
    }

    public ItemPerishMapping[] GetItemPerishMappings(ItemSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot.Itemstack);
        var stack = slot.Itemstack;

        return GetAllMappings(slot.Itemstack, null, default).ToArray();
    }

    private IEnumerable<ItemPerishMapping> GetAllMappings(ItemStack stack, ItemPerishMapping? parent, int index)
    {
        using (logger.Indent())
        {
            var current = GetMapping(stack, parent, index);

            yield return current;

            if (current.Contents.Stacks.Length == 0)
            {
                logger.Debug($"Stack {stack.GetName()} has no content");
            }

            for (int i = 0; i < current.Contents.Stacks.Length; ++i)
            {
                var subStack = current.Contents.Stacks[i];
                foreach (var subMapping in GetAllMappings(subStack, current, i))
                {
                    yield return subMapping;
                }
            }
        }
    }

    private ItemPerishMapping GetMapping(ItemStack stack, ItemPerishMapping? parent, int index)
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
            logger.Debug($"Stack {stack.GetName()} is perishable");
            transitionstate = new(stack.Attributes);
        }
        else
        {
            logger.Debug($"Stack {stack.GetName()} is not perishable");
        }

        return (contents, transitionstate, oldModData);
    }
}
