namespace Wiltoga.OfflineFoodNoSpoil;

public record ModData
{
    public required double DisconnectTotalHours { get; init; }
    public required float DisconnectFreshHours { get; init; }
}
