using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

public class Locator : ILocator
{
    private readonly Dictionary<Type, System.Func<Locator, object>> configuration = [];

    public T Get<T>() where T : class
    {
        if (configuration.TryGetValue(typeof(T), out var ctor))
        {
            return (T)ctor(this);
        }

        throw new KeyNotFoundException($"No service implemented for {typeof(T)}");
    }

    public Locator Set<TInterface, TInstance>() where TInstance : notnull, TInterface, new()
    {
        configuration[typeof(TInterface)] = _ => new TInstance();
        return this;
    }

    public Locator Set<T>(Func<T> factory) where T : notnull
    {
        configuration[typeof(T)] = _ => factory();
        return this;
    }

    public Locator Set<T>(System.Func<Locator, T> factory) where T : notnull
    {
        configuration[typeof(T)] = self => factory(self);
        return this;
    }

    private static ILocator? instance;
    public static ILocator Instance
    {
        get => instance ??= new Locator()
            .Set(() => OfflineFoodNoSpoil.Instance.Server)
            .Set<ICoreAPI>(self => self.Get<ICoreServerAPI>())
            .Set(self => self.Get<ICoreAPI>().World.Calendar)
            .Set(self => self.Get<ICoreServerAPI>().WorldManager.SaveGame)
            .Set(() => OfflineFoodNoSpoil.Instance.Mod.Info)
            .Set<ISettingsService, SettingsService>()
            .Set<IItemPerishService, ItemPerishService>()
            .Set<IModDataManager, ModDataManager>()
            .Set<IPlayerEventsHandler, PlayerEventsHandler>()
            .Set<IModLogger, ModLogger>()
            .Set<ITimeService, TimeService>()
            .Set<IInventoryScanner, InventoryScanner>()
        ;
        set => instance = value;
    }
}
