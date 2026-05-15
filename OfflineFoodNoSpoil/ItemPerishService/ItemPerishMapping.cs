using Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

namespace Wiltoga.OfflineFoodNoSpoil;

public record ItemPerishMapping
{
    public required string Key { get; init; }

    public required ContentsProxy Contents { get; init; }

    public required TransitionStateProxy? TransitionState { get; init; }

    public required ModDataProxy OldModData { get; init; }
}
