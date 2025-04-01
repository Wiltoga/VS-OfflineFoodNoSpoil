using System;

namespace Wiltoga.OfflineFoodNoSpoil
{
    public static class ConditionalLogger
    {
        public static Settings Settings { private get; set; } = default!;

        private static string Prefix => $"{OfflineFoodNoSpoil.Instance.Mod.Info.ModID} : ";

        private static int currentIndent = 0;

        private static string ComputeIndent(int value) => new string(' ', value * 2);

        public static void Debug(string message)
        {
            if (Settings.UseLogs)
            {
                OfflineFoodNoSpoil.Instance.Server.Logger.Debug(Prefix + ComputeIndent(currentIndent) + message);
            }
        }

        public static void Info(string message)
        {
            OfflineFoodNoSpoil.Instance.Server.Logger.Notification(Prefix + ComputeIndent(currentIndent) + message);
        }

        public static void Error(Exception exception)
        {
            OfflineFoodNoSpoil.Instance.Server.Logger.Error(Prefix + exception);
        }

        public static IDisposable Indent()
        {
            return new IndentScope();
        }

        private sealed class IndentScope : IDisposable
        {
            public IndentScope()
            {
                ++currentIndent;
            }

            public void Dispose()
            {
                --currentIndent;
            }
        }
    }
}
