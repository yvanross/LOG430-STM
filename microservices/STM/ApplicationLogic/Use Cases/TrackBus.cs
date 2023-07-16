using System.Collections.Concurrent;
using ApplicationLogic.Services.BusTracking;
using Entities.Transit.Interfaces;

namespace ApplicationLogic.Use_Cases;

public class TrackBus : ITrackBus
{
    private ABusTrackingService _trackingService;

    //todo use a better data structure (like a message queue maybe...)
    private readonly ConcurrentQueue<IBusTracking> _trackingQueue = new();

    private readonly PeriodicTimer _periodicTimer;

    public TrackBus(ABusTrackingService trackingService)
    {
        _trackingService = trackingService;
        _periodicTimer = new(TimeSpan.FromMilliseconds(50));

        _ = PeriodicCaller();
    }

    public void SetBus(IBus bus) => _trackingService.Bus ??= bus;

    public IBusTracking? GetUpdate()
    {
        _ = _trackingQueue.TryDequeue(out var busTracking);

        return busTracking;
    }

    private async Task PeriodicCaller()
    {
        while (await _periodicTimer.WaitForNextTickAsync())
        {
            if (_trackingService?.Bus is not null)
            {
                (var tracking, _trackingService) = _trackingService.GetUpdate();

                _trackingQueue.Enqueue(tracking);

                if (tracking.TrackingCompleted)
                {
                    _periodicTimer.Dispose();

                    return;
                }
            }
        }
    }
}