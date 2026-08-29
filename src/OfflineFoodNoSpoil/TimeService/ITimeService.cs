namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Service used for time computation of spoiling time
/// </summary>
public interface ITimeService
{
    /// <summary>
    /// Returns the spoil time to apply since the given reference, based on the settings of the mod.
    /// </summary>
    /// <param name="timeInHours"></param>
    /// <returns></returns>
    float GetSkippedTimeSince(double timeInHours);
}
