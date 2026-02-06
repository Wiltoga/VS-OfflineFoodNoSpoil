using System.Collections.Generic;
using System.Linq;

namespace Wiltoga.OfflineFoodNoSpoil;

internal static class EnumerableEx
{
    internal static float? AverageOrDefault(this IEnumerable<float> source)
    {
        if (source.Any()) return source.Average();
        return default;
    }
}
