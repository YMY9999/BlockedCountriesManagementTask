using BlockedCountriesManagement.Models;
using BlockedCountriesManagement.Service.BlockedCountries;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesManagement.Controllers
{

    public record BlockCountryRequest(string CountryCode, string Name);

    [Route("/api/countries")]
    [ApiController]
    public class BlockedCountriesController : ControllerBase
    {
        private readonly IBlockedCountriesService _blockedCountriesService;

        public BlockedCountriesController(IBlockedCountriesService blockedCountriesService)
        {
            _blockedCountriesService = blockedCountriesService;
        }

        [HttpGet("blocked")]

        public IActionResult GetBlockedCountries(
              [FromQuery] int page = 1,
              [FromQuery] int pageSize = 10,
              [FromQuery] string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var allBlocked = _blockedCountriesService.GetBlockedCountries();

            if (!string.IsNullOrEmpty(search))
            {
                var searchTerm = search.Trim().ToUpperInvariant();

                allBlocked = allBlocked.Where(c =>
                    c.Code.Contains(searchTerm) ||
                    c.Name.ToUpperInvariant().Contains(searchTerm)
                );
            }

            var totalCount = allBlocked.Count();
            var pagedResults = allBlocked
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


        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
        {
            bool wasBlocked = _blockedCountriesService.BlockCountry(request.CountryCode, request.Name);

            if (wasBlocked)
            {
                return CreatedAtAction(
                    nameof(GetBlockedCountries),
                    new { countryCode = request.CountryCode },
                    new { message = $"Country code '{request.CountryCode}' successfully blocked." }
                );
            }

            var normalizedCode = request.CountryCode?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(normalizedCode) || normalizedCode.Length != 2)
            {
                return BadRequest("Invalid country code format. Must be a 2-letter code.");
            }

            return Conflict($"Country code '{normalizedCode}' is already blocked.");
        }



        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            bool wasRemoved = _blockedCountriesService.UnblockCountry(countryCode);

            if (wasRemoved)
            {
                return NoContent();
            }

            var normalizedCode = countryCode?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(normalizedCode) || normalizedCode.Length != 2)
            {
                return BadRequest("Invalid country code format. Must be a 2-letter code.");
            }
            return NotFound($"Country code '{normalizedCode}' is not currently blocked.");
        }


        [HttpPost("temporal-block")]
        public IActionResult TemporallyBlockCountry([FromBody] TemporalBlockRequest request)
        {
            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
            {
                return BadRequest(new { message = "DurationMinutes must be between 1 and 1440 (24 hours)." });
            }

            bool wasBlocked = _blockedCountriesService.TemporallyBlockCountry(
                request.CountryCode,
                request.DurationMinutes
            );

            if (wasBlocked)
            {
                return CreatedAtAction(
                    nameof(GetBlockedCountries),
                    new { countryCode = request.CountryCode.Trim().ToUpperInvariant() },
                    new { message = $"Country code '{request.CountryCode.Trim().ToUpperInvariant()}' temporarily blocked for {request.DurationMinutes} minutes." }
                );
            }

            var normalizedCode = request.CountryCode?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(normalizedCode) || normalizedCode.Length != 2)
            {
                return BadRequest(new { message = "Invalid country code format. Must be a 2-letter code." });
            }

            return Conflict(new { message = $"Country code '{normalizedCode}' is already blocked (either permanently or temporally)." });
        }

    }
}

