using Domain.ValueObjects;

namespace Application.Dtos;

public record BusDto(
    string Name, 
    string BusId,
    Position Position);