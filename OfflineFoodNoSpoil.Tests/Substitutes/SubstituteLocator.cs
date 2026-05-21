using NSubstitute;

namespace Wiltoga.OfflineFoodNoSpoil.Tests.Substitutes;

internal class SubstituteLocator : ILocator
{
    public T Get<T>() where T : class => Substitute.For<T>();
}
