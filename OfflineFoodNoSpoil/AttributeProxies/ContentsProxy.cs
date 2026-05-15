using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

public class ContentsProxy(ITreeAttribute? attributes) : AttributeProxy(attributes)
{
    protected override string AttributeName => "contents";

    public ItemStack[] Stacks => Tree?.Values?.OfType<ItemstackAttribute>().Select(set => set.value).Where(stack => stack is not null).ToArray() ?? [];
}
