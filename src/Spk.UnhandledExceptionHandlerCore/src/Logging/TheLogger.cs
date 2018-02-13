using NLog;

namespace Spk.UnhandledExceptionHandlerCore.Logging
{
    internal static class TheLogger
    {
        public static Logger Instance { get; private set; }

        static TheLogger()
        {
            LogManager.ReconfigExistingLoggers();
            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}