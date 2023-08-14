using Domain.ValueObjects;

namespace ApplicationLogic.DTO;

public record BusDto(
    string Name, 
    string BusId,
    Position Position);