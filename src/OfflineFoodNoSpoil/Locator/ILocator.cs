namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Creates service instances
/// </summary>
public interface ILocator
{
    /// <summary>
    /// Create on instance of the requested type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Create<T>() where T : class;

    /// <summary>
    /// Create every instances of the requested type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T[] CreateAll<T>() where T : class;
}
