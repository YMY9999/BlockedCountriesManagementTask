using static BlockedCountriesManagement.Service.BlockedCountries.BlockedCountriesService;

namespace BlockedCountriesManagement.Service.BlockedCountries
{
    public interface IBlockedCountriesService
    {
        IEnumerable<BlockedCountryInfo> GetBlockedCountries();

        bool BlockCountry(string countryCode, string countryName);

        bool UnblockCountry(string countryCode);
        bool IsCountryBlocked(string countryCode);
        void Log(string message);
        IEnumerable<string> GetLogs();


        bool TemporallyBlockCountry(string countryCode, int durationMinutes);
        int CleanUpExpiredTemporalBlocks();
    }
}