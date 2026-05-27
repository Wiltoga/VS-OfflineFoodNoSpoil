using NSubstitute;
using Wiltoga.OfflineFoodNoSpoil.UnitTests.Substitutes;

namespace Wiltoga.OfflineFoodNoSpoil.UnitTests;

public class ScopedTest : IDisposable
{
    public ScopedTest()
    {
        Locator.Instance = new SubstituteLocator();
        Scope.New();

        Scope.Inject<ISettingsService>().Settings.Returns(Settings.Default);
    }
    public void Dispose()
    {
        Scope.Current.Dispose();
    }
}
