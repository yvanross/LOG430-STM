namespace Entities.Gtfs.Concretions;

public class Trip
{
    public required string Id { get; init; }
   
    public List<StopSchedule> StopSchedules { get; set; } = new ();
}