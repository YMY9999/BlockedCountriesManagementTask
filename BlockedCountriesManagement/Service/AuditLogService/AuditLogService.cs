using BlockedCountriesManagement.Models;
using System.Collections.Concurrent;

namespace BlockedCountriesManagement.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ConcurrentBag<BlockedAttemptLog> _logs = new();

        public void LogAttempt(BlockedAttemptLog log)
        {
            _logs.Add(log);
        }

        public IEnumerable<BlockedAttemptLog> GetLogs() => _logs;
    }
}