using System;
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
            var infos = scope.Get<ModInfo>();

            logger.Info($"Starting {infos.Name}");
        }

        Server.Event.PlayerJoin += Event_PlayerJoin;
        Server.Event.PlayerDisconnect += Event_PlayerDisconnect;

#if DEBUG
        GetFoodCommand.RegisterCommand(api);
#endif
    }

    private void Event_PlayerJoin(IServerPlayer byPlayer)
    {
        using var scope = Scope.New();
        var handlers = scope.GetAll<IPlayerEventsHandler>();

        foreach (var handler in handlers)
        {
            handler.PlayerJoined(byPlayer);
        }
    }

    private void Event_PlayerDisconnect(IServerPlayer byPlayer)
    {
        using var scope = Scope.New();
        var handlers = scope.GetAll<IPlayerEventsHandler>();

        foreach(var handler in handlers)
        {
            handler.PlayerDisconnected(byPlayer);
        }
    }

    public override void Dispose()
    {
        using (var scope = Scope.New())
        {
            var lifetimeServices = scope.GetAll<IServerLifetime>();

            foreach (var lifetimeService in lifetimeServices)
            {
                try
                {
                    lifetimeService.ServerStopped();
                }
                catch(Exception e)
                {
                    var logger = scope.Get<IModLogger>();
                    logger.Error(e);
                }
            }
        }
        Server.Event.PlayerJoin -= Event_PlayerJoin;
        Server.Event.PlayerDisconnect -= Event_PlayerDisconnect;
        base.Dispose();
    }
}