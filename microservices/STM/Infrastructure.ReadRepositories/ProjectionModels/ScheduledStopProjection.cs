namespace Infrastructure.ReadRepositories.ProjectionModels;

public class ScheduledStopProjection
{
    public string TripId { get; internal set; }

    public string StopId { get; internal set; }

    public DateTime DepartureTime { get; internal set; }
}