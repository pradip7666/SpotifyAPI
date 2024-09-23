using StackExchange.Profiling;

namespace SpotifyAPI.Helpers
{
    public class AuditLogger
    {
        private readonly IAuditLog _auditLog;
        private readonly MiniProfiler _miniProfiler;

        public AuditLogger(IAuditLog auditLog, MiniProfiler miniProfiler)
        {
            _auditLog = auditLog;
            _miniProfiler = miniProfiler;
        }

        public void Log(string action, string username)
        {
            using (_miniProfiler.Step("AuditLogger.Log"))
            {
                var message = $"User {username} performed {action} at {DateTime.UtcNow}";
                _auditLog.LogAudit(message);
            }
        }
    }
}
