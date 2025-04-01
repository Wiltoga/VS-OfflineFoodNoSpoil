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
            ConditionalLogger.Debug($"Item {Stack.GetName()} is perishable");
            var itemLastUpdatedTotalHours = Stack.Attributes.GetTreeAttribute("transitionstate").GetDouble("lastUpdatedTotalHours");
            var elapsedHours = world.Calendar.TotalHours - itemLastUpdatedTotalHours;
            ConditionalLogger.Debug($"Computed elapsed minutes : {elapsedHours * 60:0.##}");
            var skippedHours = elapsedHours * (1 - settings.FoodSpoilMultiplier);
            ConditionalLogger.Debug($"Skipped minutes after applying the {settings.FoodSpoilMultiplier} multiplier : {skippedHours * 60:0.##}");
            if (settings.MaxAllowedSkippedHours is not null)
            {
                ConditionalLogger.Debug($"MaxAllowedSkippedHours defined to {settings.MaxAllowedSkippedHours:0.###} (={settings.MaxAllowedSkippedHours * 60:0.##} minutes)");
                skippedHours = Math.Min(skippedHours, settings.MaxAllowedSkippedHours.Value);
            }
            ConditionalLogger.Debug($"Final skipped minutes leap : {skippedHours * 60:0.##}");
            Stack.Attributes.GetTreeAttribute("transitionstate").SetDouble("lastUpdatedTotalHours", itemLastUpdatedTotalHours + skippedHours);
        }
        else
        {
            ConditionalLogger.Debug($"Item {Stack.GetName()} is not recognized as perishable");
        }
    }

    public override string ToString()
    {
        return $"{Stack} (Perishable:{Perishable})";
    }
}
