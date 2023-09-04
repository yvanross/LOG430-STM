using Application.Commands;
using Application.Commands.Seedwork;
using Application.CommandServices.Repositories;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class LoadStaticGtfsJob : BackgroundService
{
    private readonly ILogger<LoadStaticGtfsJob> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatetimeProvider _datetimeProvider;

    private const int UpdateIntervalInHours = 6;

    public LoadStaticGtfsJob(
        IServiceProvider serviceProvider,
        IDatetimeProvider datetimeProvider,
        ILogger<LoadStaticGtfsJob> logger)
    {
        _serviceProvider = serviceProvider;
        _datetimeProvider = datetimeProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

            var eventContext = scope.ServiceProvider.GetRequiredService<IEventContext>();

            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var storedEvent = await eventContext.TryGetAsync<StaticGtfsDataLoaded>();

                if (storedEvent is null || _datetimeProvider.GetCurrentTime() - storedEvent.Created > TimeSpan.FromHours(UpdateIntervalInHours))
                {
                    _logger.LogInformation("Loading static GTFS data");

                    await commandDispatcher.DispatchAsync(new LoadStaticGtfs(), stoppingToken);

                    await publisher.Publish(new StaticGtfsDataLoaded(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));

                    _logger.LogInformation("Static GTFS data loaded");
                }
                else
                {
                    _logger.LogInformation("Static GTFS data already loaded");
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in LoadStaticGtfsService");
        }
    }
}