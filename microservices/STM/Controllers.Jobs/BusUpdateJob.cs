using Application.Commands.Seedwork;
using Application.Commands.UpdateBus;
using Application.EventHandlers.Interfaces;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class BusUpdateJob : BackgroundService
{
    private readonly ILogger<BusUpdateJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    const int BusPositionUpdateIntervalInSeconds = 10;

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

            await consumer.ConsumeNext<ServiceInitialized>(stoppingToken);

            initialScope.Dispose();

            _logger.LogInformation("Begin updating bus positions");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                await commandDispatcher.DispatchAsync(new UpdateBusesCommand(), stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(BusPositionUpdateIntervalInSeconds), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in BusUpdateService");
        }
    }
}