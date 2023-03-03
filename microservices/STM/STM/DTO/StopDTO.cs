using Entities.Concretions;
using Entities.Domain;

namespace STM.DTO;

public class StopDTO
{
    public string ID { get; set; }

    public PositionDTO Position { get; set; } = new PositionDTO();

    public string Message { get; set; }

}