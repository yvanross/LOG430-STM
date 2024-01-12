using Application.Commands.Cleaning;
using Application.Commands.LoadStaticGtfs;
using Application.Commands.Seedwork;
using Application.EventHandlers.Interfaces;
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

            var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            var staticGtfsDataLoaded = await eventContext.TryGetAsync<StaticGtfsDataLoaded>();

            var staticGtfsWasNeverLoaded = staticGtfsDataLoaded is null;

            if (staticGtfsWasNeverLoaded)
            {
                await commandDispatcher.DispatchAsync(new ClearDb(), stoppingToken);

                _logger.LogInformation("Loading static GTFS data");

                await commandDispatcher.DispatchAsync(new LoadStaticGtfsCommand(), stoppingToken);

                await publisher.Publish(new StaticGtfsDataLoaded(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));

                _logger.LogInformation("Static GTFS data loaded");
            }
            else
            {
                _logger.LogInformation("Static GTFS data already loaded");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in LoadStaticGtfsService");
        }
    }
}