using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil;

public record TransitionState(DoubleAttribute CreatedTotalHours, DoubleAttribute LastUpdatedTotalHours, FloatArrayAttribute FreshHours, FloatArrayAttribute TransitionHours, FloatArrayAttribute TransitionedHours)
{
    public static TransitionState? FromAttributes(ITreeAttribute attributes)
    {
        var transitionStateTree = attributes.GetTreeAttribute("transitionstate");
        if (transitionStateTree is null)
        {
            return null;
        }

        var freshHours = (FloatArrayAttribute)transitionStateTree["freshHours"];
        var lastUpdatedTotalHours = (DoubleAttribute)transitionStateTree["lastUpdatedTotalHours"];
        var createdTotalHours = (DoubleAttribute)transitionStateTree["createdTotalHours"];
        var transitionHours = (FloatArrayAttribute)transitionStateTree["transitionHours"];
        var transitionedHours = (FloatArrayAttribute)transitionStateTree["transitionedHours"];

        return new TransitionState(createdTotalHours, lastUpdatedTotalHours, freshHours, transitionHours, transitionedHours);
    }
}
