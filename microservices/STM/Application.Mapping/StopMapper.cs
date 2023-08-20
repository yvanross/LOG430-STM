using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Aggregates.Stop;
using Domain.Factories;

namespace Application.Mapping;

public class StopMapper : IMappingTo<IStopWrapper, Stop>
{
    public Stop MapFrom(IStopWrapper dto)
    {
        return StopFactory.Create(dto.Id, dto.Latitude, dto.Longitude);
    }
}