using Application.CommandServices.HostedServices.Processors;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices.Workers;

public class BusUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BusUpdateService> _logger;

    public BusUpdateService(IServiceProvider serviceProvider, ILogger<BusUpdateService> logger)
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
                using var scoped = _serviceProvider.CreateScope();

                var processor = scoped.ServiceProvider.GetRequiredService<BusUpdateProcessor>();

                await processor.ProcessUpdates();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in BusUpdateService");
        }
       
    }
}