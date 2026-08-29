using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

public class TransitionStateProxy(ITreeAttribute? attributes) : AttributeProxy(attributes)
{
    protected override string AttributeName => "transitionstate";

    public virtual double? LastUpdatedTotalHours { get => TryGetDouble("lastUpdatedTotalHours"); set => TrySetDouble("lastUpdatedTotalHours", value); }
    public virtual double? CreatedTotalHours { get => TryGetDouble("createdTotalHours"); set => TrySetDouble("createdTotalHours", value); }
    public virtual float[]? TransitionedHours { get => TryGetFloatArray("transitionedHours"); set => TrySetFloatArray("transitionedHours", value); }

    /// <summary>
    /// Total freshness duration of the stack
    /// </summary>
    public virtual float[]? TransitionHours { get => TryGetFloatArray("transitionHours"); set => TrySetFloatArray("transitionHours", value); }

    /// <summary>
    /// Remaining time of available freshness
    /// </summary>
    public virtual float[]? FreshHours { get => TryGetFloatArray("freshHours"); set => TrySetFloatArray("freshHours", value); }
}
