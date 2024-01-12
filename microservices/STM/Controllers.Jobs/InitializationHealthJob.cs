using Application.EventHandlers.Interfaces;
using Contracts;
using Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class InitializationHealthJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ILogger<LoadStaticGtfsJob> _logger;

    private const int MaxUpdateIntervalInHours = 12;

    public InitializationHealthJob(
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
        using var scope = _serviceProvider.CreateScope();

        var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();

        var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        
        var eventContext = scope.ServiceProvider.GetRequiredService<IEventContext>();

        if (await eventContext.TryGetAsync<StmTripModificationApplied>() is not {} @event ||
            _datetimeProvider.GetCurrentTime() - @event.Created > TimeSpan.FromHours(MaxUpdateIntervalInHours))
        {
            await consumer.ConsumeNext<StmTripModificationApplied>(stoppingToken);
        }

        await publisher.Publish(new ServiceInitialized(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));

        _logger.LogInformation("Service initialized and ready to serve requests");
    }
}