using System;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

/// <summary>
/// Old way to store data in an object. If the mod ever break, the stored data would also render
/// the item useless as it will no longer cook. Do not use this class save any info, only for reading purposes to retrieve old data.
/// </summary>
/// <param name="attributes"></param>
[Obsolete("Nothing should be stored in the item as it corrupts some parts of the game. Class still used for compatibility.")]
public class ModDataProxy(ITreeAttribute? attributes) : AttributeProxy(attributes)
{
    protected override string AttributeName => $"{OfflineFoodNoSpoil.Instance.Mod.Info.ModID}:transitionstate";

    public void DeleteData()
    {
        attributes?.RemoveAttribute(AttributeName);
    }

    public float? DisconnectTransitionedHours
    {
        get => Tree?.TryGetFloat("disconnectTransitionedHours");
    }

    public float? DisconnectFreshHours
    {
        get => Tree?.TryGetFloat("disconnectFreshHours");
    }

    public double? DisconnectTotalHours
    {
        get => Tree?.TryGetDouble("disconnectTotalHours");
    }

    public bool? Frozen
    {
        get => Tree?.TryGetBool("frozen");
    }
}
