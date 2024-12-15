using System.Numerics;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil
{
    internal static class Utils
    {
        internal static void ScaleAttribute<T>(ArrayAttribute<T> attribute, T percentage) where T : IMultiplyOperators<T, T, T>
        {
            for (int i = 0; i < attribute.value.Length; ++i)
            {
                attribute.value[i] = attribute.value[i] * percentage;
            }
        }

        internal static void ScaleAttribute<T>(ScalarAttribute<T> attribute, T percentage) where T : IMultiplyOperators<T, T, T>
        {
            attribute.value = attribute.value * percentage;
        }
    }
}
