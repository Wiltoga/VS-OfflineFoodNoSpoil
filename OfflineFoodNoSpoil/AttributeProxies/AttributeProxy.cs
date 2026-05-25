using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil;

public abstract class AttributeProxy(ITreeAttribute? attributes)
{
    protected abstract string AttributeName { get; }

    protected ITreeAttribute? Tree => attributes?.GetTreeAttribute(AttributeName);

    public virtual bool Exists => Tree is not null;
}
