using BlockedCountriesManagement.Models;

namespace BlockedCountriesManagement.Service
{
    public interface IGeolocationService
    {
        Task<IpApiResponse?> GetIpDetailsAsync(string ipAddress);
    }
}