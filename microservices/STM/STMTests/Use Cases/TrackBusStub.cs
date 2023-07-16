using ApplicationLogic.Use_Cases;
using Entities.Transit.Concretions;
using Entities.Transit.Interfaces;

namespace STMTests.Use_Cases;

public class TrackBusStub : ITrackBus
{
    private double duration = 0.0;
    
    public void SetBus(IBus bus)
    {
        //do nothing
    }

    public IBusTracking? GetUpdate()
    {
        return new BusTracking()
        {
            Duration = duration++,
            Message = $"Im a mock but here's a timer {duration}",
            TrackingCompleted = false
        };
    }
}