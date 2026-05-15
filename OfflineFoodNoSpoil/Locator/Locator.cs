using System;
using System.Collections.Generic;

namespace Wiltoga.OfflineFoodNoSpoil;

public class Locator : ILocator
{
    private readonly Dictionary<Type, Func<object>> configuration = [];

    public T Get<T>()
    {
        if (configuration.TryGetValue(typeof(T), out var ctor))
        {
            return (T)ctor();
        }

        throw new KeyNotFoundException($"No service implemented for {typeof(T)}");
    }

    public Locator Set<TInterface, TInstance>() where TInstance : TInterface, new()
    {
        configuration[typeof(TInterface)] = () => new TInstance();
        return this;
    }

    public Locator Set<T>(Func<T> factory) where T : notnull
    {
        configuration[typeof(T)] = () => factory();
        return this;
    }

    private static ILocator? instance;
    public static ILocator Instance
    {
        get => instance ??= new Locator()
            .Set(() => OfflineFoodNoSpoil.Instance.Server)
            .Set(() => OfflineFoodNoSpoil.Instance.Mod)
            .Set<ISettingsService, SettingsService>()
            .Set<IItemPerishService, ItemPerishService>()
            .Set<IModDataManager, ModDataManager>()
            .Set<IPlayerEventsHandler, PlayerEventsHandler>()
            .Set<IModLogger, ModLogger>()
            .Set<ITimeSkipService, TimeSkipService>()
            .Set<IInventoryScanner, InventoryScanner>()
        ;
        set => instance = value;
    }
}
