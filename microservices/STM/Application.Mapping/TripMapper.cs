using Application.Dtos;
using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Aggregates.Trip;
using Domain.Factories;

namespace Application.Mapping;

public class TripMapper : IMappingTo<IEnumerable<ScheduledStopDto>, Trip>, IMappingTo<Trip, IEnumerable<ScheduledStopDto>>, IMappingTo<ITripWrapper, Trip>
{
    public Trip MapFrom(IEnumerable<ScheduledStopDto> dtos)
    {
        var tripId = dtos.First().TripId;

        var scheduledStops = new List<(string id, string stopId, TimeSpan departureTimeSpan)>();

        foreach (var scheduledStopDto in dtos.OrderBy(dto => dto.StopSequence))
        {
            scheduledStops.Add((scheduledStopDto.Id, scheduledStopDto.StopId, scheduledStopDto.DepartureTimespan));
        }

        return TripFactory.Create(tripId, scheduledStops);
    }

    public IEnumerable<ScheduledStopDto> MapFrom(Trip aggregate)
    {
        return aggregate.ScheduledStops
            .Select((scheduledStop, i) => new ScheduledStopDto(
                scheduledStop.Id,
                scheduledStop.StopId,
                aggregate.Id,
                scheduledStop.ScheduledDepartureTime,
                i))
            .ToList();
    }

    public Trip MapFrom(ITripWrapper dto)
    {
        var tripId = dto.TripId;
        var scheduledStops = dto.ScheduledStops
            .OrderBy(sc => sc.StopSequence)
            .Select(scheduledStopDto => (Guid.NewGuid().ToString(), scheduledStopDto.StopId, scheduledStopDto.DepartureTime))
            .ToList();

        return TripFactory.Create(tripId, scheduledStops);
    }

}