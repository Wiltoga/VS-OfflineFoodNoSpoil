namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Interface to hook server events
/// </summary>
public interface IServerLifetime
{
    /// <summary>
    /// Triggered when the server stops
    /// </summary>
    void ServerStopped();
}
