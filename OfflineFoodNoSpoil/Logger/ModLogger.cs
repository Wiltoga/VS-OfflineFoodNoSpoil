using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class ModLogger : IModLogger
{
    private readonly ISettingsService settingsService;
    private readonly ICoreServerAPI server;
    private readonly Mod mod;
    private int currentIndent = 0;

    private string Prefix => $"{mod.Info.ModID} : ";
    
    public ModLogger()
    {
        settingsService = Scope.Inject<ISettingsService>();
        server = Scope.Inject<ICoreServerAPI>();
        mod = Scope.Inject<Mod>();
    }

    private static string ComputeIndent(int value) => new string(' ', value * 2);

    public void Debug(string message)
    {
        if (settingsService.Settings.UseLogs)
        {
            server.Logger.Debug(Prefix + ComputeIndent(currentIndent) + message);
        }
    }

    public void Info(string message)
    {
        server.Logger.Notification(Prefix + ComputeIndent(currentIndent) + message);
    }

    public void Warning(string message)
    {
        server.Logger.Warning(Prefix + ComputeIndent(currentIndent) + message);
    }

    public void Error(Exception exception)
    {
        server.Logger.Error($"{Prefix}{Environment.NewLine}Version[{mod.Info.Version}] : {exception}");
    }

    public IDisposable Indent()
    {
        return new IndentScope(this);
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
