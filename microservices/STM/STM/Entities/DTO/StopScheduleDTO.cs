using Entities.Domain;
using STM.Entities.Concretions;

namespace STM.Entities.DTO;

public class StopScheduleDTO
{
    public StopDTO? Stop { get; set; } = new StopDTO();

    public string? DepartureTime { get; set; }

    public int index { get; set; }
}