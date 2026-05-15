using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

public class OfflineFoodNoSpoil : ModSystem
{
    public ICoreServerAPI Server { get; private set; } = default!;
    public static OfflineFoodNoSpoil Instance { get; private set; } = default!;

    public OfflineFoodNoSpoil()
    {
        Instance = this;
    }

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide == EnumAppSide.Server;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        Server = api;
        using (var scope = Scope.New())
        {
            // loading to trigger the settings file creation if it doesn't exist yet
            _ = scope.Get<ISettingsService>().Settings;
            var logger = scope.Get<IModLogger>();

            logger.Info($"Starting {Mod.Info.Name}");
        }

        Server.Event.PlayerJoin += Event_PlayerJoin;
        Server.Event.PlayerDisconnect += Event_PlayerDisconnect;
    }

    private void Event_PlayerJoin(IServerPlayer byPlayer)
    {
        using var scope = Scope.New();
        var handler = scope.Get<IPlayerEventsHandler>();

        handler.PlayerJoined(byPlayer);
    }

    private void Event_PlayerDisconnect(IServerPlayer byPlayer)
    {
        using var scope = Scope.New();
        var handler = scope.Get<IPlayerEventsHandler>();

        handler.PlayerDisconnected(byPlayer);
    }

    public override void Dispose()
    {
        base.Dispose();
        Server.Event.PlayerJoin -= Event_PlayerJoin;
        Server.Event.PlayerDisconnect -= Event_PlayerDisconnect;
    }
}