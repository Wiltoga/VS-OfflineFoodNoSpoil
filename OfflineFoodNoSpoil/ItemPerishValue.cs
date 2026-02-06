using System;
using System.Linq;
using Vintagestory.API.Common;
using Wiltoga.OfflineFoodNoSpoil.Attributes;

namespace Wiltoga.OfflineFoodNoSpoil;

public class ItemPerishValue
{
    public Contents? Contents { get; }

    public Transitionstate? PerishState { get; }

    public ModData? ModData { get; }

    public ItemStack? Stack { get; }

    public ItemPerishValue(ItemStack? stack)
    {
        if (stack is null)
        {
            return;
        }

        Stack = stack;

        if (stack.Collectible?.TransitionableProps?.Any(property => property.Type == EnumTransitionType.Perish) is not null)
        {
            PerishState = new(stack.Attributes);
            ModData = new(stack.Attributes);
        }

        Contents = new(stack.Attributes);
    }

    public bool SkipElapsedHours(IWorldAccessor world, Settings settings)
    {
        if (PerishState is not null && ModData is not null)
        {
            var itemLastUpdatedTotalHours = ModData.DisconnectTotalHours ?? PerishState.LastUpdatedTotalHours;
            if (itemLastUpdatedTotalHours is null)
            {
                return false;
            }
            var elapsedHours = world.Calendar.TotalHours - itemLastUpdatedTotalHours.Value;
            ConditionalLogger.Debug($"Computed elapsed minutes : {elapsedHours * 60:0.##}");
            var skippedHours = elapsedHours * (1 - settings.FoodSpoilMultiplier);
            ConditionalLogger.Debug($"Skipped minutes after applying the {settings.FoodSpoilMultiplier} multiplier : {skippedHours * 60:0.##}");
            if (settings.MaxAllowedSkippedHours is not null)
            {
                ConditionalLogger.Debug($"MaxAllowedSkippedHours defined to {settings.MaxAllowedSkippedHours:0.###} (={settings.MaxAllowedSkippedHours * 60:0.##} minutes)");
                skippedHours = Math.Min(skippedHours, settings.MaxAllowedSkippedHours.Value);
            }
            ConditionalLogger.Debug($"Final skipped minutes leap : {skippedHours * 60:0.##}");
            PerishState.LastUpdatedTotalHours = itemLastUpdatedTotalHours + skippedHours;

            return true;
        }
        return false;
    }

    public bool SnapState(IWorldAccessor world, Settings settings)
    {
        if (PerishState is not null && ModData is not null)
        {
            ModData.DisconnectTotalHours = world.Calendar.TotalHours;
            ModData.DisconnectTransitionedHours = PerishState.TransitionedHours;
            ModData.DisconnectFreshHours = PerishState.FreshHours;
            return true;
        }
        return false;
    }

    public void FreezeTime(IWorldAccessor world, Settings settings)
    {
        if (PerishState is not null && ModData is not null)
        {
            // setting food to way higher fresh hour to hold its freeze state
            PerishState.FreshHours = 10000000;
            PerishState.TransitionedHours = 0;
        }
    }

    public void UnFreezeTime(IWorldAccessor world, Settings settings)
    {
        if (PerishState is not null && ModData is not null)
        {
            if (ModData.DisconnectFreshHours is not null)
            {
                PerishState.FreshHours = ModData.DisconnectFreshHours;
            }
            if (ModData.DisconnectTransitionedHours is not null)
            {
                PerishState.TransitionedHours = ModData.DisconnectTransitionedHours;
            }
        }
    }
}
