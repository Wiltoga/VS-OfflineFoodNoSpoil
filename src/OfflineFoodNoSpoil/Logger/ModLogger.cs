using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class ModLogger : IModLogger, IServerLifetime
{
    private readonly ISettingsService settingsService;
    private readonly ICoreAPI api;
    private readonly ModInfo modInfo;
    private int currentIndent = 0;

    /// <summary>
    /// List of crash dumps created in order to delete them on server stop
    /// </summary>
    private static readonly ConcurrentBag<string> crashDumps = [];

    /// <summary>
    /// Prefix of every logged message
    /// </summary>
    private string Prefix => $"{modInfo.ModID} : ";
    
    public ModLogger()
    {
        settingsService = Scope.Inject<ISettingsService>();
        api = Scope.Inject<ICoreAPI>();
        modInfo = Scope.Inject<ModInfo>();
    }

    private static string ComputeIndent(int value) => new string(' ', value * 2);

    public void Debug(string message)
    {
        if (settingsService.Settings.UseLogs)
        {
            api.Logger.Debug(Prefix + ComputeIndent(currentIndent) + message);
        }
    }

    public void Info(string message)
    {
        api.Logger.Notification(Prefix + ComputeIndent(currentIndent) + message);
    }

    public void Warning(string message)
    {
        api.Logger.Warning(Prefix + ComputeIndent(currentIndent) + message);
    }

    public void Error(string message)
    {
        api.Logger.Error(Prefix + ComputeIndent(currentIndent) + message);
    }

    public void Error(Exception exception)
    {
        api.Logger.Error($"{Prefix}{Environment.NewLine}Version[{modInfo.Version}] : {exception}");
    }

    public string? CreateCrashDump(string text)
    {
        if (settingsService.Settings.CreateCrashDumps)
        {
            try
            {
                var filename = Path.GetTempFileName();
                using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    writer.WriteLine($"Version[{modInfo.Version}]");
                    writer.Write(text);
                }
                crashDumps.Add(filename);
                return filename;
            }
            catch(Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public IDisposable Indent()
    {
        return new IndentScope(this);
    }

    public void ServerStopped()
    {
        foreach (var crashDump in crashDumps)
        {
            try
            {
                File.Delete(crashDump);
            }
            catch { }
        }
    }

    private sealed class IndentScope : IDisposable
    {
        private readonly ModLogger logger;

        public IndentScope(ModLogger logger)
        {
            ++logger.currentIndent;
            this.logger = logger;
        }

        public void Dispose()
        {
            --logger.currentIndent;
        }
    }
}
