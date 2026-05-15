using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class ItemPerishService : IItemPerishService
{
    private readonly IModLogger logger;

    public ItemPerishService()
    {
        logger = Scope.Inject<IModLogger>();
    }

    public void FreezeItem(IInventory inventory, ItemPerishMapping item)
    {
        // todo: freeze item
    }

    public void UnfreezeItem(IInventory inventory, ItemPerishMapping item)
    {
        // todo: unfreeze item
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
