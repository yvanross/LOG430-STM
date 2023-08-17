using Application.Mapping.Interfaces.Wrappers;
using Application.Mapping.Interfaces;
using Domain.Aggregates;
using Domain.Factories;
using Domain.ValueObjects;

namespace Application.Mapping;

public class StopMapper : IMappingTo<IStopWrapper, Stop>
{
    public Stop MapFrom(IStopWrapper dto)
    {
        return StopFactory.Create(dto.Id, new Position(dto.Latitude, dto.Longitude));
    }
}