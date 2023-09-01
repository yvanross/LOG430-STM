﻿using Application.Commands;
using Application.Commands.Seedwork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Controllers.Jobs;

public class RideTrackingJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RideTrackingJob> _logger;

    public RideTrackingJob(IServiceProvider serviceProvider, ILogger<RideTrackingJob> logger)
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

            while (!stoppingToken.IsCancellationRequested)
            {
                await commandDispatcher.DispatchAsync(new UpdateRideTracking(), stoppingToken);

                await Task.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in RideTrackingService");
        }
      
    }
}