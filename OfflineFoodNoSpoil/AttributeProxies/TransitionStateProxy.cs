using System.Linq;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

public class TransitionStateProxy(ITreeAttribute? attributes) : AttributeProxy(attributes)
{
    protected override string AttributeName => "transitionstate";

    public virtual double? LastUpdatedTotalHours
    {
        get => Tree?.TryGetDouble("lastUpdatedTotalHours");
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("lastUpdatedTotalHours"))
                {
                    Tree.RemoveAttribute("lastUpdatedTotalHours");
                }
                else if (value is not null)
                {
                    Tree.SetDouble("lastUpdatedTotalHours", value.Value);
                }
            }
        }
    }

    public virtual double? CreatedTotalHours
    {
        get => Tree?.TryGetDouble("createdTotalHours");
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("createdTotalHours"))
                {
                    Tree.RemoveAttribute("createdTotalHours");
                }
                else if (value is not null)
                {
                    Tree.SetDouble("createdTotalHours", value.Value);
                }
            }
        }
    }

    public virtual float[]? TransitionedHours
    {
        get => (Tree?["transitionedHours"] as FloatArrayAttribute)?.value;
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("transitionedHours"))
                {
                    Tree.RemoveAttribute("transitionedHours");
                }
                else if (value is not null)
                {
                    Tree["transitionedHours"] = new FloatArrayAttribute(value);
                }
            }
        }
    }

    /// <summary>
    /// Total freshness duration of the stack
    /// </summary>
    public virtual float[]? TransitionHours
    {
        get => (Tree?["transitionHours"] as FloatArrayAttribute)?.value;
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("transitionHours"))
                {
                    Tree.RemoveAttribute("transitionHours");
                }
                else if (value is not null)
                {
                    Tree["transitionHours"] = new FloatArrayAttribute(value);
                }
            }
        }
    }

    /// <summary>
    /// Remaining time of available freshness
    /// </summary>
    public virtual float[]? FreshHours
    {
        get => (Tree?["freshHours"] as FloatArrayAttribute)?.value;
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("freshHours"))
                {
                    Tree.RemoveAttribute("freshHours");
                }
                else if (value is not null)
                {
                    Tree["freshHours"] = new FloatArrayAttribute(value);
                }
            }
        }
    }
}
