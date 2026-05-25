using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Wiltoga.OfflineFoodNoSpoil;

public sealed class Scope : IDisposable
{
    private readonly Dictionary<Type, object?> cache = [];

    public static ThreadLocal<Scope> current = new();
    public static Scope Current { get => current.Value!; private set => current.Value = value; }
    public static T Inject<T>() where T : class => Current.Get<T>();

    private Scope()
    {
        Current = this;
    }
    
    public static Scope New() => new Scope();

    public void Dispose()
    {
        foreach (var disposable in cache.Values.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
        Current = null!;
    }

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

            return (T)(cache[typeof(T)] = Locator.Instance.Get<T>()!);
        }
    }
}
