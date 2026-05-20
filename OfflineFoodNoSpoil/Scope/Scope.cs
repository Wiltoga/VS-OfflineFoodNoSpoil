using System;
using System.Collections.Generic;
using System.Linq;

namespace Wiltoga.OfflineFoodNoSpoil;

public sealed class Scope : IScope
{
    private readonly Dictionary<Type, object?> cache = [];

    public static IScope Current { get; private set; } = default!;
    public static T Inject<T>() where T : class => Current.Get<T>();

    private Scope()
    {
        Current = this;
    }
    
    public static IScope New() => new Scope();

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
