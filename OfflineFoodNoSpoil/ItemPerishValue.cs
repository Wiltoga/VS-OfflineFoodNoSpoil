using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil;

public class ItemPerishValue
{
    public ItemPerishValue[] Content { get; }
    public bool Perishable { get; }
    public ItemStack Stack { get; }

    public ItemPerishValue(ItemStack stack)
    {
        Stack = stack;

        if (stack.Collectible.TransitionableProps is not null)
        {
            Perishable = Array.Exists(stack.Collectible.TransitionableProps, property => property.Type == EnumTransitionType.Perish);
        }
        else
        {
            Perishable = false;
        }

        var contents = stack.Attributes.GetTreeAttribute("contents");
        if (contents is not null)
        {
            var listContent = new List<ItemPerishValue>();
            foreach (var set in contents.Values.OfType<ItemstackAttribute>())
            {
                listContent.Add(new ItemPerishValue(set.value));
            }
            Content = listContent.ToArray();
        }
        else
        {
            Content = Array.Empty<ItemPerishValue>();
        }
    }

    public void ResetUpdatedTotalHours(IWorldAccessor world, Settings settings)
    {
        if (Perishable)
        {
            var itemLastUpdatedTotalHours = Stack.Attributes.GetTreeAttribute("transitionstate").GetDouble("lastUpdatedTotalHours");
            var elapsedHours = world.Calendar.TotalHours - itemLastUpdatedTotalHours;
            elapsedHours *= settings.FoodSpoilMultiplier;
            Stack.Attributes.GetTreeAttribute("transitionstate").SetDouble("lastUpdatedTotalHours", world.Calendar.TotalHours - elapsedHours);
        }
    }

    public override string ToString()
    {
        return $"{Stack} (Perishable:{Perishable})";
    }
}
