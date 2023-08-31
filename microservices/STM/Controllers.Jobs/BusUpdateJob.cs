using Application.Commands;
using Application.Commands.Seedwork;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class BusUpdateJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BusUpdateJob> _logger;

    public BusUpdateJob(IServiceProvider serviceProvider, ILogger<BusUpdateJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var initialScope = _serviceProvider.CreateScope();

            var consumer = initialScope.ServiceProvider.GetRequiredService<IConsumer>();

            await consumer.ConsumeNext<StaticGtfsDataLoaded>(stoppingToken);

            initialScope.Dispose();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                await commandDispatcher.DispatchAsync(new UpdateBuses(), stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in BusUpdateService");
        }
       
    }
}