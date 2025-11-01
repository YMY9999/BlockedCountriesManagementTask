using System.Collections.Concurrent;

namespace BlockedCountriesManagement.Service.BlockedCountries
{
    public class BlockedCountriesService : IBlockedCountriesService
    {
        public record BlockedCountryInfo(string Code, string Name);

        private readonly ConcurrentDictionary<string, BlockedCountryInfo> _blockedCountries = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentBag<string> _logs = new();
        private readonly ConcurrentDictionary<string, DateTime> _temporalBlocks = new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<BlockedCountryInfo> GetBlockedCountries() => _blockedCountries.Values;

        public bool BlockCountry(string countryCode, string countryName)
        {
            var normalizedCode = countryCode?.Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(normalizedCode) || normalizedCode.Length != 2) return false;
            if (string.IsNullOrEmpty(countryName)) return false;

            var info = new BlockedCountryInfo(normalizedCode, countryName.Trim());

            return _blockedCountries.TryAdd(normalizedCode, info);
        }

        public bool UnblockCountry(string countryCode)
        {
            var normalizedCode = countryCode?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(normalizedCode) || normalizedCode.Length != 2)
            {
                return false;
            }
            return _blockedCountries.TryRemove(normalizedCode, out _);
        }

        public bool IsCountryBlocked(string countryCode)
        {
            var normalizedCode = countryCode?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(normalizedCode)) return false;
            if (_blockedCountries.ContainsKey(normalizedCode))
            {
                return true;
            }
            if (_temporalBlocks.TryGetValue(normalizedCode, out DateTime expiryTime))
            {
                if (expiryTime > DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    _temporalBlocks.TryRemove(normalizedCode, out _);
                    return false;
                }
            }
            return false;
        }

        public bool TemporallyBlockCountry(string countryCode, int durationMinutes)
        {
            var normalizedCode = countryCode?.Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(normalizedCode) || normalizedCode.Length != 2) return false;
            if (durationMinutes < 1 || durationMinutes > 1440) return false;

            if (_blockedCountries.ContainsKey(normalizedCode))
            {
                return false;
            }

            var expiryTime = DateTime.UtcNow.AddMinutes(durationMinutes);

            return _temporalBlocks.TryAdd(normalizedCode, expiryTime);
        }

        public int CleanUpExpiredTemporalBlocks()
        {
            int removedCount = 0;
            var now = DateTime.UtcNow;

            var keysToCheck = _temporalBlocks.Keys;

            foreach (var key in keysToCheck)
            {
                if (_temporalBlocks.TryGetValue(key, out DateTime expiryTime) && expiryTime <= now)
                {
                    if (_temporalBlocks.TryRemove(key, out _))
                    {
                        removedCount++;
                    }
                }
            }
            return removedCount;
        }

        public void Log(string message)
        {
            var timestampedMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}";
            _logs.Add(timestampedMessage);
        }

        public IEnumerable<string> GetLogs() => _logs;
    }
}