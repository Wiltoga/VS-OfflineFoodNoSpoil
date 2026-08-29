namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Structure of the saved data for one perish entry
/// </summary>
public sealed record ModData
{
    /// <summary>
    /// The calendar time in hours of the disconnection of the player
    /// </summary>
    public required double DisconnectTotalHours { get; init; }
}
