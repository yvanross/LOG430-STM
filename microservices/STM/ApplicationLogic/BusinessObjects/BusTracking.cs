namespace ApplicationLogic.BusinessObjects;

public class BusTracking
{
    public required string Message { get; set; }

    public bool TrackingCompleted { get; set; }

    public double Duration { get; set; }
}