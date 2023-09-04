using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Aggregates.Trip;
using Domain.Factories;

namespace Application.Mapping;

public class TripMapper : IMappingTo<ITripWrapper, Trip>
{
    public Trip MapFrom(ITripWrapper dto)
    {
        return TripFactory.Create(dto.TripId,
            dto.ScheduledStops.Select(scheduledStop => (scheduledStop.StopId, scheduledStop.DepartureTime)));
    }
}