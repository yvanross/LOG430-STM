namespace Application.Dtos;

public class ScheduledStopDto
{
    public string Id { get; set; }

    public string StopId { get; set; } 

    public string TripId { get; set; }

    public TimeSpan DepartureTimespan { get; set; }

    public int StopSequence { get; set; }
}