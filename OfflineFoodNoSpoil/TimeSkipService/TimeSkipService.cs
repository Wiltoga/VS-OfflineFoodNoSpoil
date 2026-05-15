using System;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class TimeSkipService : ITimeSkipService
{
    private readonly ICoreServerAPI server;
    private readonly IModLogger logger;
    private readonly Settings settings;

    public TimeSkipService()
    {
        server = Scope.Inject<ICoreServerAPI>();
        logger = Scope.Inject<IModLogger>();
        settings = Scope.Inject<ISettingsService>().Settings;
    }

    public double GetSkippedTime(double hourReference)
    {
        var elapsedHours = server.World.Calendar.TotalHours - hourReference;
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
