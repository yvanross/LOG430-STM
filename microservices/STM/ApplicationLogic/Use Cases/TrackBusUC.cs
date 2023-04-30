using System.Collections.Concurrent;
using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services.BusTracking;
using Entities.Domain;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Use_Cases;

public class TrackBusUC
{
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMilliseconds(10));

    //todo use a better data structure
    private readonly ConcurrentQueue<IBusTracking> _trackingQueue = new();

    private ITrackingService? _trackingService;

    public TrackBusUC(IBus bus, double eta, IStmClient stmClient, ILogger? logger)
    {
        _trackingService = new BeforeFirstStopTrackingService(bus, stmClient, logger, eta);

        _ = PeriodicCaller();
    }

    public async Task PeriodicCaller()
    {
        while (await _periodicTimer.WaitForNextTickAsync())
        {
            await Try.WithConsequenceAsync(() =>
            {
                if (_trackingService is not null)
                {
                    (IBusTracking tracking, ITrackingService? service) = _trackingService.GetUpdate();

                    _trackingService = service;
                    _trackingQueue.Enqueue(tracking);

                    if (tracking.TrackingCompleted)
                        _periodicTimer.Dispose();
                }

                return Task.FromResult(0);
            }, retryCount: 10);
        }
    }

    public IBusTracking? GetUpdate()
    {
        _ = _trackingQueue.TryDequeue(out var busTracking);

        return busTracking;
    }
}