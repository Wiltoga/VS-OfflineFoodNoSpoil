using System;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IModLogger
{
    void Debug(string message);
    void Error(Exception exception);
    IDisposable Indent();
    void Info(string message);
}
