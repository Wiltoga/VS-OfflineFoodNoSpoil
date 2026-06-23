using NSubstitute;

namespace Wiltoga.OfflineFoodNoSpoil.UnitTests.Substitutes;

internal class SubstituteLocator : ILocator
{
    public T Create<T>() where T : class => Substitute.For<T>();
    public T[] CreateAll<T>() where T : class => [Create<T>()];
}
