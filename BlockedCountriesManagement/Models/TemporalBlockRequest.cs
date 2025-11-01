namespace BlockedCountriesManagement.Models
{
    public record TemporalBlockRequest(string CountryCode, int DurationMinutes);
}