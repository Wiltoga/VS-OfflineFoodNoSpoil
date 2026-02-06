using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.Attributes;

public class Contents(ITreeAttribute attributes)
{
    private const string AttributeName = "contents";
    private ITreeAttribute? Tree => attributes?.GetTreeAttribute(AttributeName);

    public ItemStack?[] Stacks => Tree?.Values?.OfType<ItemstackAttribute>().Select(set => set.value).ToArray() ?? [];
}
