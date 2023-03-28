using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class BusTracking : IBusTracking
{
    public string Message { get; set; }

    public bool TrackingCompleted { get; set; }

    public int Duration { get; set; }
}