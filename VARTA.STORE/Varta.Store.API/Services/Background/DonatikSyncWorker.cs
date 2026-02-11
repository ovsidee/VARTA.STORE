using Varta.Store.API.Services.Interfaces;

namespace Varta.Store.API.Services.Background;

public class DonatikSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DonatikSyncWorker> _logger;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(1); // Run every minute

    public DonatikSyncWorker(IServiceProvider serviceProvider, ILogger<DonatikSyncWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[DonatikSyncWorker] Service starting...");

        // Run immediately on start
        try
        {
            await PerformSyncAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[DonatikSyncWorker] Initial sync failed.");
        }

        using var timer = new PeriodicTimer(_period);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await PerformSyncAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DonatikSyncWorker] Error during sync cycle.");
            }
        }
    }

    private async Task PerformSyncAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[DonatikSyncWorker] Checking for new donations...");

        using (var scope = _serviceProvider.CreateScope())
        {
            var donatikService = scope.ServiceProvider.GetRequiredService<IDonatikService>();

            try
            {
                // Fetch recent donations (last 100 or so)
                var donations = await donatikService.GetRecentDonationsAsync(100);

                int incomingCount = donations.Count;
                int processedCount = 0;

                foreach (var donation in donations)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    // ProcessDonationAsync handles idempotency, so it's safe to call repeatedly
                    var result = await donatikService.ProcessDonationAsync(donation);
                    if (result.Success)
                    {
                        processedCount++;
                    }
                    else
                    {
                        _logger.LogWarning($"[DonatikSyncWorker] Failed to process {donation.Id}: {result.Message}");
                    }
                }

                if (processedCount > 0)
                {
                    _logger.LogInformation($"[DonatikSyncWorker] Specific sync cycle processed {processedCount} new donations.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DonatikSyncWorker] Failed to fetch or process donations.");
            }
        }
    }
}
