using BlockedCountriesManagement.Models;
using BlockedCountriesManagement.Service;
using BlockedCountriesManagement.Service.BlockedCountries;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BlockedCountriesManagement.Controllers
{
    [Route("api/ip")]
    [ApiController]
    public class IpLookupController : ControllerBase
    {

        private readonly IGeolocationService _geolocationService;
        private readonly IBlockedCountriesService _blockedCountriesService;
        private readonly IAuditLogService _auditLogService;

        public IpLookupController(
            IGeolocationService geolocationService,
            IBlockedCountriesService blockedCountriesService,
            IAuditLogService auditLogService)
        {
            _geolocationService = geolocationService;
            _blockedCountriesService = blockedCountriesService;
            _auditLogService = auditLogService;

        }


        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIp([FromQuery] string? ipAddress)
        {
            string targetIp;

            if (!string.IsNullOrEmpty(ipAddress))
            {
                targetIp = ipAddress;
            }
            else
            {
                var remoteIp = HttpContext.Connection.RemoteIpAddress;

                if (remoteIp == null)
                {
                    return BadRequest(new { message = "IP address not provided and caller IP could not be determined." });
                }

                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }
                targetIp = remoteIp.ToString();
            }

            if (!IPAddress.TryParse(targetIp, out _))
            {
                return BadRequest(new { message = "Invalid IP address format.", ip = targetIp });
            }

            var ipDetails = await _geolocationService.GetIpDetailsAsync(targetIp);

            if (ipDetails == null)
            {
                return StatusCode(503, new { message = "Invalid IP address format.", ip = targetIp });
            }

            return Ok(ipDetails);
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress;
            string targetIp;

            if (remoteIp == null)
            {
                return BadRequest(new { message = "Caller IP could not be determined." });
            }

            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }
            targetIp = remoteIp.ToString();

            // targetIp is giving me a lot of trouble and can't read my ip address correctly 
            // So For testing purposes, override with a specific IP
            targetIp = "154.178.250.39";

            string? userAgent = Request.Headers["User-Agent"].ToString();

            var ipDetails = await _geolocationService.GetIpDetailsAsync(targetIp);

            if (ipDetails == null || !string.IsNullOrEmpty(ipDetails.Message))
            {
                var failedLog = new BlockedAttemptLog(DateTime.UtcNow, targetIp, "UNKNOWN", false, userAgent);
                _auditLogService.LogAttempt(failedLog);

                return StatusCode(503, new { message = "Geolocation service is unavailable or failed." });
            }

            var countryCode = ipDetails.CountryCode;

            bool isBlocked = false;
            if (!string.IsNullOrEmpty(countryCode))
            {
                isBlocked = _blockedCountriesService.IsCountryBlocked(countryCode);
            }

            var log = new BlockedAttemptLog(DateTime.UtcNow, targetIp, countryCode, isBlocked, userAgent);
            _auditLogService.LogAttempt(log);

            return Ok(new
            {
                ipAddress = targetIp,
                countryCode = countryCode,
                countryName = ipDetails.CountryName,
                isBlocked = isBlocked
            });
        }
    }
}
