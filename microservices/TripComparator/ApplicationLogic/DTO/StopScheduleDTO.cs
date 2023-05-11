namespace TripComparator.DTO;

public class StopScheduleDto
{
    public StopDto? Stop { get; set; } = new ();

    public string? DepartureTime { get; set; }

    public int Index { get; set; }
}