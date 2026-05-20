using NSubstitute;

namespace Wiltoga.OfflineFoodNoSpoil.Tests;

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
