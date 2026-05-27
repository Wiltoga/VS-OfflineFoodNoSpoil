using System;

namespace Wiltoga.OfflineFoodNoSpoil;

public interface IModLogger
{
    void Debug(string message);
    void Error(Exception exception);
    void Error(string message);
    void Warning(string message);
    IDisposable Indent();
    void Info(string message);
}
