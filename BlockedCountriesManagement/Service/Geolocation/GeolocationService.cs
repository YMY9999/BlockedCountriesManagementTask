using BlockedCountriesManagement.Models;

namespace BlockedCountriesManagement.Service
{
    public class GeolocationService : IGeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeolocationService> _logger;
        private readonly string _apiKey;

        public GeolocationService(
            HttpClient httpClient,
            ILogger<GeolocationService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;

            _apiKey = configuration["GeolocationApi:ApiKey"]
                      ?? throw new InvalidOperationException("GeolocationApi:ApiKey not configured.");
        }

        public async Task<IpApiResponse?> GetIpDetailsAsync(string ipAddress)
        {
            string requestUrl = $"ipgeo?apiKey={_apiKey}&ip={ipAddress}";

            _logger.LogInformation("Attempting to query Geolocation API at: {Base}{Request}", _httpClient.BaseAddress, requestUrl);

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Geolocation API failed with status {StatusCode} at {Url}", response.StatusCode, response.RequestMessage?.RequestUri);
                    return null;
                }

                var ipInfo = await response.Content.ReadFromJsonAsync<IpApiResponse>();

                if (!string.IsNullOrEmpty(ipInfo?.Message))
                {
                    _logger.LogWarning("Geolocation API returned an error: {Reason}", ipInfo.Message);
                    return ipInfo;
                }

                return ipInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown calling Geolocation API for IP {IP}", ipAddress);
                return null;
            }
        }
    }
}