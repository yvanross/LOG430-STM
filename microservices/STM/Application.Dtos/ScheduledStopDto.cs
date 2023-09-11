namespace Application.Dtos;

public class ScheduledStopDto
{
    public ScheduledStopDto(string id, string stopId, string tripId, TimeSpan departureTimespan, int stopSequence)
    {
        Id = id;
        StopId = stopId;
        TripId = tripId;
        DepartureTimespan = departureTimespan;
        StopSequence = stopSequence;
    }

    public string Id { get; set; }

    public string StopId { get; set; } 

    public string TripId { get; set; }

    public TimeSpan DepartureTimespan { get; set; }

    public int StopSequence { get; set; }
}