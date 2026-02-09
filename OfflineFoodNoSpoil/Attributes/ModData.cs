using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.Attributes;

public class ModData(ITreeAttribute? attributes)
{
    private static string AttributeName => $"{OfflineFoodNoSpoil.Instance.Mod.Info.ModID}:transitionstate";
    private ITreeAttribute? Tree
    {
        get
        {
            if (attributes?.TryGetAttribute(AttributeName, out var tree) is true)
            {
                return tree as ITreeAttribute;
            }
            return null;
        }
        
    }

    public void DeleteData()
    {
        attributes?.RemoveAttribute(AttributeName);
    }

    private void SetupData()
    {
        attributes?.GetOrAddTreeAttribute(AttributeName);
    }

    public float? DisconnectTransitionedHours
    {
        get => Tree?.TryGetFloat("disconnectTransitionedHours");
        set
        {
            SetupData();
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
            SetupData();
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
            SetupData();
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

    public bool? Frozen
    {
        get => Tree?.TryGetBool("frozen");
        set
        {
            SetupData();
            if (Tree is not null)
            {
                if (value is null && Tree.HasAttribute("frozen"))
                {
                    Tree.RemoveAttribute("frozen");
                }
                else if (value is not null)
                {
                    Tree.SetBool("frozen", value.Value);
                }
            }
        }
    }
}
