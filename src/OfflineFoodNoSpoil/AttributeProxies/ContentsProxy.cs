using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil.AttributeProxies;

public class ContentsProxy(ITreeAttribute? attributes) : AttributeProxy(attributes)
{
    protected override string AttributeName => "contents";

    /// <summary>
    /// The list of stacks in this attribute tree
    /// </summary>
    public virtual Dictionary<string, ItemStack> Stacks => Tree?
        .Where(pair => pair.Value is ItemstackAttribute { value: not null })
        .ToDictionary(
            pair => pair.Key,
            pair => ((ItemstackAttribute)pair.Value).value)
        ?? [];
}
