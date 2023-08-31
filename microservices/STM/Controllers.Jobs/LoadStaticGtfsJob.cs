using Application.Commands;
using Application.Commands.Seedwork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class LoadStaticGtfsJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoadStaticGtfsJob> _logger;

    public LoadStaticGtfsJob(IServiceProvider serviceProvider, ILogger<LoadStaticGtfsJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

            await commandDispatcher.DispatchAsync(new LoadStaticGtfs(), stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in LoadStaticGtfsService");
        }
       
    }
}