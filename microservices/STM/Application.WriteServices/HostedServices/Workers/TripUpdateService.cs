using Application.CommandServices.HostedServices.Processors;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices.Workers;

public class TripUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TripUpdateService> _logger;

    public TripUpdateService(IServiceProvider serviceProvider, ILogger<TripUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();

            await consumer.ConsumeNext<StaticGtfsDataLoaded>(stoppingToken);

            var processor = scope.ServiceProvider.GetRequiredService<TripUpdateProcessor>();

            while (!stoppingToken.IsCancellationRequested)
            {
                await processor.ProcessUpdates();

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in TripUpdateService");
        }
       
    }
}