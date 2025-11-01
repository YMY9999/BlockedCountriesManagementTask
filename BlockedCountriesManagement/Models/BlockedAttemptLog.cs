namespace BlockedCountriesManagement.Models
{
    public record BlockedAttemptLog(
        DateTime Timestamp,
        string IpAddress,
        string? CountryCode,
        bool WasBlocked,
        string? UserAgent
    );
}