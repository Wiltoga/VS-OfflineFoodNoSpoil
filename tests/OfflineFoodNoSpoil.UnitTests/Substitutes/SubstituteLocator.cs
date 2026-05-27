using NSubstitute;

namespace Wiltoga.OfflineFoodNoSpoil.UnitTests.Substitutes;

internal class SubstituteLocator : ILocator
{
    public T Get<T>() where T : class => Substitute.For<T>();
}
