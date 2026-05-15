using System;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IScope : IDisposable
{
    T Get<T>();

    abstract static IScope New();
}
