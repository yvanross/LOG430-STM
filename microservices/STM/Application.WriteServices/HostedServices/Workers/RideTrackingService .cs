using Application.CommandServices.HostedServices.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices.Workers;

public class RideTrackingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RideTrackingService> _logger;

    public RideTrackingService(IServiceProvider serviceProvider, ILogger<RideTrackingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var processor = scope.ServiceProvider.GetRequiredService<RideTrackingProcessor>();

            while (!stoppingToken.IsCancellationRequested)
            {
                await processor.ProcessUpdates();

                await Task.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in RideTrackingService");
        }
      
    }
}