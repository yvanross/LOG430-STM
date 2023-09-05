using Application.Commands.LoadStaticGtfs;
using Application.Commands.Seedwork;
using Application.Commands.UpdateStaticGtfs;
using Application.EventHandlers;
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

    private const double UpdateIntervalInHours = 0.01;//6;

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
                var staticGtfsDataLoaded = await eventContext.TryGetAsync<StaticGtfsDataLoaded>();
                var staticGtfsDataUpdated = await eventContext.TryGetAsync<StaticGtfsDataUpdated>();

                var staticGtfsWasNeverLoaded = staticGtfsDataLoaded is null;

                var staticGtfsWasLoadedButLongAgo = _datetimeProvider.GetCurrentTime() - (staticGtfsDataUpdated as Event ?? staticGtfsDataLoaded)?.Created > TimeSpan.FromHours(UpdateIntervalInHours);

                if (staticGtfsWasNeverLoaded)
                {
                    _logger.LogInformation("Loading static GTFS data");

                    await commandDispatcher.DispatchAsync(new LoadStaticGtfsCommand(), stoppingToken);

                    await publisher.Publish(new StaticGtfsDataLoaded(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));

                    _logger.LogInformation("Static GTFS data loaded");
                }
                else if (staticGtfsWasLoadedButLongAgo)
                {
                    _logger.LogInformation("Updating static GTFS data");

                    await commandDispatcher.DispatchAsync(new UpdateStaticGtfsCommand(), stoppingToken);

                    await publisher.Publish(new StaticGtfsDataUpdated(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));

                    _logger.LogInformation("Static GTFS data updated");
                }
                else
                {
                    _logger.LogInformation("Static GTFS data already loaded");
                }

                await Task.Delay(TimeSpan.FromHours(UpdateIntervalInHours), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in LoadStaticGtfsService");
        }
    }
}