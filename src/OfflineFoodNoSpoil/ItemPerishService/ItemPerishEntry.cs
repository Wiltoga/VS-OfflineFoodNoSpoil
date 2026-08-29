using System;
using Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// One perishable entry of an inventory slot
/// </summary>
/// VS handles all different contents of a perishable item separately.
/// One perishable item can have multiple perishable parts, for example a pie has all its ingredients separately tracked.
/// One of these tracked ingredient is a perishable entry.
public record ItemPerishEntry
{
    /// <summary>
    /// Unique key generated to save data and retrieve it.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Unique generated name. Used for debugging purposes.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The content proxy of the attributes of this entry
    /// </summary>
    public required ContentsProxy Contents { get; init; }

    /// <summary>
    /// The transitionstate proxy of the attributes of this entry
    /// </summary>
    public required TransitionStateProxy? TransitionState { get; init; }

    /// <summary>
    /// A proxy to the attributes of this entry, used to retrieve data saved in a deprecated way (from mod version 2.0.1-dev1)
    /// </summary>
    [Obsolete("Remove this at some point, only there of old compatibility")]
    public required ModDataProxy OldModData { get; init; }
}
