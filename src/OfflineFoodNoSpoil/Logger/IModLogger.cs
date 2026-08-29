using System;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Wrapper to the server logger
/// </summary>
public interface IModLogger
{
    /// <summary>
    /// Logs a message as debug. Only logs if the settings allow it.
    /// </summary>
    /// <param name="message"></param>
    void Debug(string message);

    /// <summary>
    /// Logs an exception as an error
    /// </summary>
    /// <param name="exception"></param>
    void Error(Exception exception);

    /// <summary>
    /// Logs a message as an error
    /// </summary>
    /// <param name="message"></param>
    void Error(string message);

    /// <summary>
    /// Logs a message as a warning
    /// </summary>
    /// <param name="message"></param>
    void Warning(string message);

    /// <summary>
    /// Logs a message as info
    /// </summary>
    /// <param name="message"></param>
    void Info(string message);

    /// <summary>
    /// Begin an identation scope in the logs
    /// </summary>
    /// <returns></returns>
    IDisposable Indent();

    /// <summary>
    /// Creates a crash dump for debugging purposes. Only creates it if the settings allow it.
    /// </summary>
    /// <param name="text">Text to dump</param>
    /// <returns>The file path if the dump is created</returns>
    string? CreateCrashDump(string text);
}
