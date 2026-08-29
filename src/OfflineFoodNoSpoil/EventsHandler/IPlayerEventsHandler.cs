using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Service interface used to hook to player events
/// </summary>
public interface IPlayerEventsHandler
{
    /// <summary>
    /// Triggered on player join
    /// </summary>
    /// <param name="byPlayer"></param>
    void PlayerJoined(IServerPlayer byPlayer);

    /// <summary>
    /// Triggered on player disconnect
    /// </summary>
    /// <param name="byPlayer"></param>
    void PlayerDisconnected(IServerPlayer byPlayer);
}
