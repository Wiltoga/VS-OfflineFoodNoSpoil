using NSubstitute;

namespace Wiltoga.OfflineFoodNoSpoil.Tests;

internal class SubstituteLocator : ILocator
{
    public T Get<T>() where T : class => Substitute.For<T>();
}
