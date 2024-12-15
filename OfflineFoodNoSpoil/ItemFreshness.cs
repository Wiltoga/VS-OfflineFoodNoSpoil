using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil;

public class ItemFreshness
{
    public TransitionState? TransitionState { get; }
    public TransitionState[] Content { get; }

    public ItemStack Stack { get; }

    public IEnumerable<TransitionState> AllStates => Content.Append(TransitionState).OfType<TransitionState>();

    public bool HasFreshness => AllStates.Any();

    public float AverageFreshHours => AllStates.SelectMany(attribute => attribute.FreshHours.value).Average();
    public double AverageCreatedTotalHours => AllStates.Select(attribute => attribute.CreatedTotalHours.value).Average();
    public double AverageLastUpdatedTotalHours => AllStates.Select(attribute => attribute.LastUpdatedTotalHours.value).Average();
    public float AverageTransitionedHours => AllStates.SelectMany(attribute => attribute.TransitionedHours.value).Average();
    public float AverageTransitionHours => AllStates.SelectMany(attribute => attribute.TransitionHours.value).Average();

    public float AverageFreshness
    {
        get => 1 - (AverageTransitionedHours / AverageFreshHours);
        set
        {
            foreach (var attribute in AllStates)
            {
                Utils.ScaleAttribute(attribute.TransitionedHours, 1 - value);
            }
        }
    }

    public ItemFreshness(ItemStack stack)
    {
        Stack = stack;

        TransitionState = TransitionState.FromAttributes(stack.Attributes);

        var contents = stack.Attributes.GetTreeAttribute("contents");
        if (contents is not null)
        {
            var listContent = new List<TransitionState>();
            foreach (var set in contents.Values.OfType<ItemstackAttribute>())
            {
                var contentTransitionState = TransitionState.FromAttributes(set.value.Attributes);
                if (contentTransitionState is not null)
                    listContent.Add(contentTransitionState);
            }
            Content = listContent.ToArray();
        }
        else
        {
            Content = Array.Empty<TransitionState>();
        }
    }

    public override string ToString()
    {
        return $"{Stack} (HasFreshness:{HasFreshness})";
    }
}
