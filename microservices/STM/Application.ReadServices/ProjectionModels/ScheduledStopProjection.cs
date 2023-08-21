namespace Application.QueryServices.ProjectionModels;

public class ScheduledStopProjection
{
    public string Id { get; set; }

    public string TripId { get; set; }

    public string StopId { get; set; }

    public DateTime DepartureTime { get; set; }
}