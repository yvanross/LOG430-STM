using ApplicationLogic.DTO;
using Domain.Aggregates;
using Domain.Entities;
using Domain.Factories;

namespace Application.Mapping;

internal static class RideMapper
{
    internal static Ride Map(BusDto busDto, Trip trip, int currentStopIndex, ScheduledStop source, ScheduledStop destination)
    {
        return RideFactory.CreateRide(busDto.BusId, busDto.Name, busDto.Position, currentStopIndex, trip, source, destination);
    }
}