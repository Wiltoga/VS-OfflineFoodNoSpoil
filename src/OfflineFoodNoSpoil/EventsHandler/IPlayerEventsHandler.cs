using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IPlayerEventsHandler
{
    void PlayerJoined(IServerPlayer byPlayer);

    void PlayerDisconnected(IServerPlayer byPlayer);
}
