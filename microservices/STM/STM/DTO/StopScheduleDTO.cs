namespace STM.Dto;

public class StopScheduleDto
{
    public StopDto? Stop { get; set; } = new ();

    public string? DepartureTime { get; set; }

    public int index { get; set; }
}