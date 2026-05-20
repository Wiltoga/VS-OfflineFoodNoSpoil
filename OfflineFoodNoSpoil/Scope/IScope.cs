using System;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IScope : IDisposable
{
    T Get<T>() where T : class;

    abstract static IScope New();
}
