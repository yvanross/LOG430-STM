using Entities.Domain;

namespace STM.DTO;

public class StopScheduleDTO
{
    public StopDTO? Stop { get; set; } = new StopDTO();

    public string? DepartureTime { get; set; }

    public int index { get; set; }
}