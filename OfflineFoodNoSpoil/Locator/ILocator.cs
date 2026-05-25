namespace Wiltoga.OfflineFoodNoSpoil;

public interface ILocator
{
    T Get<T>() where T : class;
}
