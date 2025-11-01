using BlockedCountriesManagement.Models;

namespace BlockedCountriesManagement.Service
{
    public interface IAuditLogService
    {
        void LogAttempt(BlockedAttemptLog log);
        IEnumerable<BlockedAttemptLog> GetLogs();

    }
}