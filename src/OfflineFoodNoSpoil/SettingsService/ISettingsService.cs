namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Service accessing saved mod settings
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets the settings of the mod
    /// </summary>
    Settings Settings { get; }
}
