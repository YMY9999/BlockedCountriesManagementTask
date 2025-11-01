using BlockedCountriesManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesManagement.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public LogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var allLogs = _auditLogService.GetLogs()
                                         .OrderByDescending(log => log.Timestamp);

            var totalCount = allLogs.Count();

            var pagedResults = allLogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var response = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = pagedResults
            };

            return Ok(response);
        }
    }
}