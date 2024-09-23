using Serilog;
using StackExchange.Profiling;

namespace SpotifyAPI.Helpers
{
    public static class AuditLog
    {
        public static void LogInfo(string message)
        {
            using (MiniProfiler.Current.Step(Constants.LogInfoMessage))
            {
                Log.Information(message);
            }
        }

        public static void LogError(string message, Exception ex)
        {
            using (MiniProfiler.Current.Step(Constants.LogErrorMessage))
            {
                Log.Error(ex, message);
            }
        }

        public static void LogAudit(string message)
        {
            using (MiniProfiler.Current.Step(Constants.LogAuditMessage))
            {
                Log.Information("AUDIT: " + message);
            }
        }
    }
}

    