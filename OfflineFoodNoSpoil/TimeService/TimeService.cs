using System;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class TimeService : ITimeService
{
    private readonly IGameCalendar calendar;
    private readonly IModLogger logger;
    private readonly ISettingsService settingsService;

    public TimeService()
    {
        calendar = Scope.Inject<IGameCalendar>();
        logger = Scope.Inject<IModLogger>();
        settingsService = Scope.Inject<ISettingsService>();
    }

    public float GetSkippedTimeSince(double hourReference)
    {
        var settings = settingsService.Settings;

        var elapsedHours = (float)(calendar.TotalHours - hourReference);
        logger.Debug($"Computed elapsed minutes : {elapsedHours * 60:0.##}");
        var skippedHours = elapsedHours * (1 - settings.FoodSpoilMultiplier);
        logger.Debug($"Skipped minutes after applying the {settings.FoodSpoilMultiplier} multiplier : {skippedHours * 60:0.##}");
        if (settings.MaxAllowedSkippedHours is not null)
        {
            logger.Debug($"MaxAllowedSkippedHours defined to {settings.MaxAllowedSkippedHours:0.###} (={settings.MaxAllowedSkippedHours * 60:0.##} minutes)");
            skippedHours = Math.Min(skippedHours, settings.MaxAllowedSkippedHours.Value);
        }
        logger.Debug($"Final skipped minutes leap : {skippedHours * 60:0.##}");
        return skippedHours;
    }
}
