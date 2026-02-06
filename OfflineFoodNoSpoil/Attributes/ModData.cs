using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.Attributes;

public class ModData(ITreeAttribute? attributes)
{
    private static string AttributeName => $"{OfflineFoodNoSpoil.Instance.Mod.Info.ModID}:transitionstate";
    private ITreeAttribute? Tree => attributes?.GetOrAddTreeAttribute(AttributeName);

    public float? DisconnectTransitionedHours
    {
        get => Tree?.TryGetFloat("disconnectTransitionedHours");
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("disconnectTransitionedHours"))
                {
                    Tree.RemoveAttribute("disconnectTransitionedHours");
                }
                else if (value is not null)
                {
                    Tree.SetFloat("disconnectTransitionedHours", value.Value);
                }
            }
        }
    }

    public float? DisconnectFreshHours
    {
        get => Tree?.TryGetFloat("disconnectFreshHours");
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("disconnectFreshHours"))
                {
                    Tree.RemoveAttribute("disconnectFreshHours");
                }
                else if (value is not null)
                {
                    Tree.SetFloat("disconnectFreshHours", value.Value);
                }
            }
        }
    }

    public double? DisconnectTotalHours
    {
        get => Tree?.TryGetDouble("disconnectTotalHours");
        set
        {
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("disconnectTotalHours"))
                {
                    Tree.RemoveAttribute("disconnectTotalHours");
                }
                else if (value is not null)
                {
                    Tree.SetDouble("disconnectTotalHours", value.Value);
                }
            }
        }
    }
}
