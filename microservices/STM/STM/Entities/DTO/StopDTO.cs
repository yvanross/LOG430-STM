using Entities.Concretions;
using Entities.Domain;
using STM.Entities.Domain;

namespace STM.Entities.DTO;

public class StopDTO
{
    public string ID { get; set; }
    
    public PositionDTO Position { get; set; } = new PositionDTO();

    public string Message { get; set; }

}