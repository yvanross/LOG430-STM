using Application.Commands;
using Application.Commands.Seedwork;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class UpdateTripsJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UpdateTripsJob> _logger;

    public UpdateTripsJob(IServiceProvider serviceProvider, ILogger<UpdateTripsJob> logger)
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

            await consumer.ConsumeNext<StaticGtfsDataLoaded>(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await commandDispatcher.DispatchAsync(new UpdateTrips(), stoppingToken);

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in TripUpdateService");
        }
       
    }
}