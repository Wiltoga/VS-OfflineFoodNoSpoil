using AwesomeAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil.UnitTests;

public class SettingsServiceShould : ScopedTest
{
    private readonly SettingsService service;
    private readonly ICoreServerAPI server;

    public SettingsServiceShould()
    {
        service = new();
        server = Scope.Inject<ICoreServerAPI>();
    }

    [Fact]
    public void NotLoadIfNotNeeded()
    {
        // service created in ctor

        server.DidNotReceive().LoadModConfig<Settings>(service.SettingsFile);
    }

    [Fact]
    public void LoadOnce()
    {
        _ = service.Settings;
        _ = service.Settings;

        server.Received(1).LoadModConfig<Settings>(service.SettingsFile);
    }

    [Fact]
    public void UseDefaultIfFileNotFound()
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Returns(null as Settings);

        var storedData = service.Settings;

        storedData.Should().BeEquivalentTo(Settings.Default);
    }

    [Fact]
    public void UseDefaultIfLoadingFailed()
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Throws<Exception>();

        var storedData = service.Settings;

        storedData.Should().BeEquivalentTo(Settings.Default);
    }

    [Fact]
    public void CreateDefaultIfFileNotFound()
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Returns(null as Settings);

        _ = service.Settings;

        server.Received(1).StoreModConfig(Settings.Default, service.SettingsFile);
    }

    [Fact]
    public void CreateDefaultIfLoadingFailed()
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Throws<Exception>();

        _ = service.Settings;

        server.Received(1).StoreModConfig(Settings.Default, service.SettingsFile);
    }

    [Fact]
    public void AlwaysSaveSettings()
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Returns(Settings.Default with
        {
            MaxAllowedSkippedHours = 5f,
        });

        var settings = service.Settings;

        server.Received(1).StoreModConfig(settings, service.SettingsFile);
    }

    [Theory]
    [InlineData(-0.1f)]
    [InlineData(float.NaN)]
    public void ValidateMaxAllowedSkippedHours(float? maxAllowed)
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Returns(Settings.Default with
        {
            MaxAllowedSkippedHours = maxAllowed,
        });

        var fixedSettings = service.Settings;

        fixedSettings.MaxAllowedSkippedHours.Should().BeNull();
    }

    [Theory]
    [InlineData(-0.1f, 0f)]
    [InlineData(1.1f, 1f)]
    [InlineData(float.PositiveInfinity, 1f)]
    [InlineData(float.NegativeInfinity, 0f)]
    [InlineData(float.NaN, 0f)]
    public void ValidateFoodSpoilMultiplier(float multiplier, float expectedFix)
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Returns(Settings.Default with
        {
            FoodSpoilMultiplier = multiplier,
        });

        var fixedSettings = service.Settings;

        fixedSettings.FoodSpoilMultiplier.Should().Be(expectedFix);
    }

    public static TheoryData<string[]> ValidateInventoriesBlacklistData => new(
        null!,
        [],
        [null!]);

    [Theory]
    [MemberData(nameof(ValidateInventoriesBlacklistData))]
    public void ValidateInventoriesBlacklist(string[] blackList)
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Returns(Settings.Default with
        {
            InventoriesBlacklist = blackList,
        });

        var fixedSettings = service.Settings;

        fixedSettings.InventoriesBlacklist.Should().BeEquivalentTo(Settings.Default.InventoriesBlacklist);
    }

    [Fact]
    public void FilterNullInventoriesBlacklist()
    {
        server.LoadModConfig<Settings>(service.SettingsFile).Returns(Settings.Default with
        {
            InventoriesBlacklist = ["1", null!, "2"],
        });

        var fixedSettings = service.Settings;

        fixedSettings.InventoriesBlacklist.Should().BeEquivalentTo("1", "2");
    }
}
