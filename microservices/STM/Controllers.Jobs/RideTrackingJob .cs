using Application.Commands;
using Application.Commands.Seedwork;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class RideTrackingJob : BackgroundService
{
    private readonly ILogger<RideTrackingJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RideTrackingJob(IServiceProvider serviceProvider, ILogger<RideTrackingJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();

            await consumer.ConsumeNext<ServiceInitialized>(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await commandDispatcher.DispatchAsync(new UpdateRideTracking(), stoppingToken);

                await Task.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in RideTrackingService");
        }
    }
}