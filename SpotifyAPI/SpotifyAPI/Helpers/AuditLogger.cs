using StackExchange.Profiling;

namespace SpotifyAPI.Helpers
{
    public class AuditLogger
    {
        public void Log(string action, string username)
        {
            using (MiniProfiler.Current.Step("AuditLogger.Log"))
            {
                var message = $"User {username} performed {action} at {DateTime.UtcNow}";
                AuditLog.LogAudit(message);
            }
        }
    }
}
