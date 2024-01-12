using Application.Commands.Seedwork;
using Application.Commands.UpdateTrips;
using Application.EventHandlers.Interfaces;
using Contracts;
using Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class UpdateTripsJob : BackgroundService
{
    private readonly ILogger<UpdateTripsJob> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatetimeProvider _datetimeProvider;

    private const int MaxUpdateIntervalInHours = 6;

    public UpdateTripsJob(IServiceProvider serviceProvider, IDatetimeProvider datetimeProvider, ILogger<UpdateTripsJob> logger)
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

            var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();
          
            var eventContext = scope.ServiceProvider.GetRequiredService<IEventContext>();

            consumer.Subscribe<StaticGtfsDataLoaded>(async (_, token) =>
            {
                await DispatchUpdateTrips(commandDispatcher, token, publisher);
            }, _logger);

            await ApplyImmediateUpdateIfRequired(stoppingToken, eventContext, commandDispatcher, publisher);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in TripUpdateService");
        }
    }

    private async Task ApplyImmediateUpdateIfRequired(
        CancellationToken stoppingToken,
        IEventContext eventContext,
        ICommandDispatcher commandDispatcher,
        IEventPublisher eventPublisher)
    {
        var priorEvent = await eventContext.TryGetAsync<StaticGtfsDataLoaded>();

        var @event = await eventContext.TryGetAsync<StmTripModificationApplied>();

        var staticGtfsWasLoadedButNotUpdated = priorEvent is not null && @event is null;

        var staticGtfsWasLoadedAndUpdatedButLongAgo = priorEvent is not null && @event is not null &&
                                                      _datetimeProvider.GetCurrentTime() - @event.Created >
                                                      TimeSpan.FromHours(MaxUpdateIntervalInHours);

        if (staticGtfsWasLoadedButNotUpdated || staticGtfsWasLoadedAndUpdatedButLongAgo)
        {
            await DispatchUpdateTrips(commandDispatcher, stoppingToken, eventPublisher);
        }
    }

    private async Task DispatchUpdateTrips(ICommandDispatcher commandDispatcher, CancellationToken token, IEventPublisher eventPublisher)
    {
        await commandDispatcher.DispatchAsync(new UpdateTripsCommand(), token);

        await eventPublisher.Publish(new StmTripModificationApplied(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));

        _logger.LogInformation("Trips updated");
    }
}