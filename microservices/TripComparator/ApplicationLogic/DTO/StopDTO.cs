namespace TripComparator.DTO;

public class StopDto
{
    public string Id { get; set; }

    public PositionDto Position { get; set; } = new PositionDto();

    public string Message { get; set; }

}