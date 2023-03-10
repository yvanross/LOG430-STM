namespace STM.Dto;

public class StopDto
{
    public string ID { get; set; }

    public PositionDto Position { get; set; } = new PositionDto();

    public string Message { get; set; }

}