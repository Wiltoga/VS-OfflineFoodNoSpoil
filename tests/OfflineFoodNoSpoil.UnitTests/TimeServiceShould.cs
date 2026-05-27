using AwesomeAssertions;
using NSubstitute;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil.UnitTests;

public class TimeServiceShould : ScopedTest
{
    private readonly TimeService service;
    private readonly IGameCalendar calendar;
    private readonly ISettingsService settingsService;

    public TimeServiceShould()
    {
        service = new();
        calendar = Scope.Inject<IGameCalendar>();
        settingsService = Scope.Inject<ISettingsService>();
    }

    [Theory]
    [InlineData(0.5f, 9f, 9f)]
    [InlineData(0.5f, 15f, 10f)]
    [InlineData(0.2f, null, 16f)]
    [InlineData(0f, 5f, 5f)]
    public void ComputeWithFoodMultiplierAndMaxAllowedHours(float foodSpoilMultiplier, float? maxAllowedSkippedHours, float expectedSkippedHours)
    {
        calendar.TotalHours.Returns(30);
        settingsService.Settings.Returns(new Settings
        {
            MaxAllowedSkippedHours = maxAllowedSkippedHours,
            FoodSpoilMultiplier = foodSpoilMultiplier,
        });

        var skippedTime = service.GetSkippedTimeSince(10);

        skippedTime.Should().Be(expectedSkippedHours);
    }
}
