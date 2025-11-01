using System.Text.Json.Serialization;

namespace BlockedCountriesManagement.Models
{
    public record IpApiResponse
    {
        [JsonPropertyName("ip")]
        public string Ip { get; init; } = string.Empty;

        [JsonPropertyName("country_code2")]
        public string? CountryCode { get; init; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; init; }

        [JsonPropertyName("isp")]
        public string? Isp { get; init; }

        [JsonPropertyName("message")]
        public string? Message { get; init; }
    }
}