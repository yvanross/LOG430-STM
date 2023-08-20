using Application.CommandServices.HostedServices.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices.Workers;

public class LoadStaticGtfsService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoadStaticGtfsService> _logger;

    public LoadStaticGtfsService(IServiceProvider serviceProvider, ILogger<LoadStaticGtfsService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scoped = _serviceProvider.CreateScope();

            var processor = scoped.ServiceProvider.GetRequiredService<LoadStaticGtfsProcessor>();

            await processor.ProcessUpdates();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in LoadStaticGtfsService");
        }
       
    }
}