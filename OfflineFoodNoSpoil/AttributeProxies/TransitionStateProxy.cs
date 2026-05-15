using System.Linq;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

public class TransitionStateProxy(ITreeAttribute? attributes) : AttributeProxy(attributes)
{
    protected override string AttributeName => "transitionstate";

    public double? LastUpdatedTotalHours
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

    public double? CreatedTotalHours
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

    public float? TransitionedHours
    {
        get => (Tree?["transitionedHours"] as FloatArrayAttribute)?.value.AverageOrDefault();
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
                    var count = (Tree["transitionedHours"] as FloatArrayAttribute)?.value.Length ?? 1;
                    Tree["transitionedHours"] = new FloatArrayAttribute(Enumerable.Repeat(value.Value, count).ToArray());
                }
            }
        }
    }

    public float? TransitionHours
    {
        get => (Tree?["transitionHours"] as FloatArrayAttribute)?.value.AverageOrDefault();
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
                    var count = (Tree["transitionHours"] as FloatArrayAttribute)?.value.Length ?? 1;
                    Tree["transitionHours"] = new FloatArrayAttribute(Enumerable.Repeat(value.Value, count).ToArray());
                }
            }
        }
    }

    public float? FreshHours
    {
        get => (Tree?["freshHours"] as FloatArrayAttribute)?.value.AverageOrDefault();
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
                    var count = (Tree["freshHours"] as FloatArrayAttribute)?.value.Length ?? 1;
                    Tree["freshHours"] = new FloatArrayAttribute(Enumerable.Repeat(value.Value, count).ToArray());
                }
            }
        }
    }
}
