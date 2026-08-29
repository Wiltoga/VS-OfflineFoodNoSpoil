using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Represents a service scope execution
/// </summary>
/// <remarks>
/// Used to reuse services already created from cache in this scope
/// </remarks>
public sealed class Scope : IDisposable
{
    private readonly Dictionary<Type, object?> cache = [];

    /// <summary>
    /// Backup field of the current scope in the current thread (if the game every handles player events in multiple threads)
    /// </summary>
    public static ThreadLocal<Scope> current = new();
    public static Scope Current { get => current.Value!; private set => current.Value = value; }

    /// <summary>
    /// Retrieve a service from the current scope's cache or create a new service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Inject<T>() where T : class => Current.Get<T>();

    private Scope()
    {
        Current = this;
    }
    
    /// <summary>
    /// Creates a new service scope if it doesn't already exist
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Scope New()
    {
        if (Current is null)
        {
            return new();
        }
        throw new InvalidOperationException("A scope is already created.");
    }

    public void Dispose()
    {
        // Dispose all created services in this scope
        foreach (var disposable in cache.Values.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
        Current = null!;
    }

    /// <summary>
    /// Retrieve a service from the cache or create a new service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T Get<T>() where T : class
    {
        if (cache.TryGetValue(typeof(T), out var t))
        {
            if (t is null)
            {
                throw new InvalidOperationException($"Dependency loop detected when requesting {typeof(T)}");
            }
            return (T)t;
        }
        else
        {
            // pre-allocating the slot to detect loops
            cache[typeof(T)] = null;

            return (T)(cache[typeof(T)] = Locator.Instance.Create<T>()!);
        }
    }

    /// <summary>
    /// Retrieve all services for the requested type from the cache or create new services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T[] GetAll<T>() where T : class
    {
        if (cache.TryGetValue(typeof(T[]), out var t))
        {
            if (t is null)
            {
                throw new InvalidOperationException($"Dependency loop detected when requesting {typeof(T)}");
            }
            return (T[])t;
        }
        else
        {
            // pre-allocating the slot to detect loops
            cache[typeof(T[])] = null;

            return (T[])(cache[typeof(T[])] = Locator.Instance.CreateAll<T>()!);
        }
    }
}
