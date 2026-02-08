using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Services.Contracts;

namespace UrGuide.WebApp.Services
{
    /// <summary>
    /// Background service that processes pending data export requests and cleans up expired exports
    /// </summary>
    public class DataExportBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataExportBackgroundService> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(6);
        private DateTime _lastCleanup = DateTime.MinValue;

        public DataExportBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<DataExportBackgroundService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Export Background Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Process pending exports
                    await ProcessPendingExportsAsync(stoppingToken);

                    // Cleanup expired exports (every 6 hours)
                    if (DateTime.UtcNow - _lastCleanup > _cleanupInterval)
                    {
                        await CleanupExpiredExportsAsync(stoppingToken);
                        _lastCleanup = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Data Export Background Service");
                }

                // Wait before next iteration
                await Task.Delay(_processingInterval, stoppingToken);
            }

            _logger.LogInformation("Data Export Background Service is stopping");
        }

        private async Task ProcessPendingExportsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dataExportService = scope.ServiceProvider.GetRequiredService<IDataExportService>();

            try
            {
                var processedCount = await dataExportService.ProcessPendingExportsAsync(cancellationToken);
                
                if (processedCount > 0)
                {
                    _logger.LogInformation("Processed {Count} pending data exports", processedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process pending data exports");
            }
        }

        private async Task CleanupExpiredExportsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dataExportService = scope.ServiceProvider.GetRequiredService<IDataExportService>();

            try
            {
                var cleanedCount = await dataExportService.CleanupExpiredExportsAsync(cancellationToken);
                
                if (cleanedCount > 0)
                {
                    _logger.LogInformation("Cleaned up {Count} expired data exports", cleanedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup expired data exports");
            }
        }
    }
}
