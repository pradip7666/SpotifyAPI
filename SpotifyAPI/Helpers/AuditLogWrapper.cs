using StackExchange.Profiling;

namespace SpotifyAPI.Helpers
{
    public interface IAuditLog
    {
        void LogAudit(string message);
    }

    public class AuditLogWrapper : IAuditLog
    {
        private readonly IAuditLog _auditLog;

        public AuditLogWrapper(IAuditLog auditLog)
        {
            _auditLog = auditLog;
        }

        public void LogAudit(string message)
        {
            _auditLog.LogAudit(message);
        }
    }
}
