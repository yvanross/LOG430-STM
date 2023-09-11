using Application.Commands.Seedwork;
using Application.Commands.UpdateRidesTracking;
using Application.EventHandlers.Interfaces;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class RideTrackingJob : BackgroundService
{
    private readonly ILogger<RideTrackingJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    private const int UpdateIntervalInMs = 50;

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

            _logger.LogInformation($"Begin updating rides tracking, the command logging is muted for this one as the interval of {UpdateIntervalInMs}ms would clutter the console");

            while (!stoppingToken.IsCancellationRequested)
            {
                await commandDispatcher.DispatchAsync(new UpdateRidesTrackingCommand(), stoppingToken);

                await Task.Delay(TimeSpan.FromMilliseconds(UpdateIntervalInMs), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in RideTrackingService");
        }
    }
}