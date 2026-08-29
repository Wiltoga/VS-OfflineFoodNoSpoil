using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

public class Locator : ILocator
{
    /// <summary>
    /// A configuration of one service
    /// </summary>
    private sealed record Service
    {
        /// <summary>
        /// The type to index the service
        /// </summary>
        public required Type Type { get; init; }

        /// <summary>
        /// The factory of the configured service
        /// </summary>
        public required System.Func<Locator, object> Factory { get; init; }
    }
    private readonly List<Service> configuration = [];

    public T Create<T>() where T : class
    {
        var service = configuration.LastOrDefault(service => service.Type == typeof(T));
        if (service is not null)
        {
            return (T)service.Factory(this);
        }

        throw new KeyNotFoundException($"No service implemented for {typeof(T)}");
    }

    /// <summary>
    /// Configures a type for a requestable type
    /// </summary>
    /// <typeparam name="TInterface">The type to retrieve the service</typeparam>
    /// <typeparam name="TInstance">The actual type of the created service</typeparam>
    /// <returns></returns>
    public Locator Set<TInterface, TInstance>() where TInstance : notnull, TInterface, new() where TInterface : notnull
    {
        return Set<TInterface>(_ => new TInstance());
    }

    /// <summary>
    /// Configures a factory for a requestable type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    public Locator Set<T>(Func<T> factory) where T : notnull
    {
        return Set(_ => factory());
    }

    /// <summary>
    /// Configures a factory for a requestable type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    public Locator Set<T>(System.Func<Locator, T> factory) where T : notnull
    {
        configuration.Add(new()
        {
            Type = typeof(T),
            Factory = self => factory(self),
        });
        return this;
    }

    public T[] CreateAll<T>() where T : class
    {
        var services = configuration.Where(service => service.Type == typeof(T)).ToArray();
        return services.Select(service => (T)service.Factory(this)).ToArray();
    }

    private static ILocator? instance;

    /// <summary>
    /// The instance of the locator used to create services.
    /// </summary>
    /// <remarks>
    /// Can be overriden for unit tests
    /// </remarks>
    public static ILocator Instance
    {
        get => instance ??= new Locator()
            .Set(() => OfflineFoodNoSpoil.Instance.Server)
            .Set<ICoreAPI>(self => self.Create<ICoreServerAPI>())
            .Set(self => self.Create<ICoreAPI>().World.Calendar)
            .Set(self => self.Create<ICoreServerAPI>().WorldManager.SaveGame)
            .Set(() => OfflineFoodNoSpoil.Instance.Mod.Info)
            .Set<ISettingsService, SettingsService>()
            .Set<IItemPerishService, ItemPerishService>()
            .Set<IModDataManager, ModDataManager>()
            .Set<IPlayerEventsHandler, PlayerEventsHandler>()
            .Set<IModLogger, ModLogger>()
            .Set<IServerLifetime, ModLogger>()
            .Set<ITimeService, TimeService>()
            .Set<IInventorySnapper, InventorySnapper>()
        ;
        internal set => instance = value;
    }
}
