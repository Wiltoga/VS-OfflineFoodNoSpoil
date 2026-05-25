using System;
using Vintagestory.API.Common;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class ModLogger : IModLogger
{
    private readonly ISettingsService settingsService;
    private readonly ICoreAPI api;
    private readonly Mod mod;
    private int currentIndent = 0;

    private string Prefix => $"{mod.Info.ModID} : ";
    
    public ModLogger()
    {
        settingsService = Scope.Inject<ISettingsService>();
        api = Scope.Inject<ICoreAPI>();
        mod = Scope.Inject<Mod>();
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
        api.Logger.Error($"{Prefix}{Environment.NewLine}Version[{mod.Info.Version}] : {exception}");
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
