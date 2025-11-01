using BlockedCountriesManagement.Service.BlockedCountries;

namespace BlockedCountriesManagement.BackgroundServices
{
    public class TemporalBlockCleanupService : BackgroundService
    {
        private readonly ILogger<TemporalBlockCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public TemporalBlockCleanupService(
            ILogger<TemporalBlockCleanupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Temporal Block Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);

                    _logger.LogInformation("Running temporal block cleanup task...");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var blockedCountriesService =
                            scope.ServiceProvider.GetRequiredService<IBlockedCountriesService>();

                        int cleanedCount = blockedCountriesService.CleanUpExpiredTemporalBlocks();

                        if (cleanedCount > 0)
                        {
                            _logger.LogInformation("Cleaned up {Count} expired temporal blocks.", cleanedCount);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Temporal Block Cleanup Service.");
                }
            }

            _logger.LogInformation("Temporal Block Cleanup Service is stopping.");
        }
    }
}