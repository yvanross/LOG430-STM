using Entities.Transit.Interfaces;

namespace Entities.Transit.Concretions;

public class BusTracking : IBusTracking
{
    public string Message { get; set; }

    public bool TrackingCompleted { get; set; }

    public double Duration { get; set; }
}