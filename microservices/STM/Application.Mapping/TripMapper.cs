using Application.Mapping.Interfaces;
using Domain.Aggregates;
using Domain.Common.Interfaces;
using Domain.Entities;
using STM.ExternalServiceProvider.Proto;

namespace Application.Mapping;

public class TripMapper : IMappingTo<TripUpdate, Trip>
{
    private readonly IDatetimeProvider _datetimeProvider;

    public TripMapper(IDatetimeProvider datetimeProvider)
    {
        _datetimeProvider = datetimeProvider;
    }

    public Trip MapTo(TripUpdate dto)
    {
        var id = dto.Trip.TripId;

        var scheduledStops => dto.Vehicle.

        var stopSchedule = dto.StopTimeUpdate.ToList().ConvertAll(stopTimeUpdate =>
            {
                if (_stops!.ContainsKey(stopTimeUpdate.StopId) is false)
                    AddStop(new Stop() { Id = stopTimeUpdate.StopId, Position = new PositionLL() });

                var schedule = new ScheduledStop(stopTimeUpdate.StopId, (DateTime.UnixEpoch.AddSeconds(stopTimeUpdate.Departure?.Time ?? stopTimeUpdate.Arrival?.Time ?? 0L)).AddHours(_datetimeProvider.GetUtcDifference()))
                {
                    StopId = _stops[stopTimeUpdate.StopId],
                    DepartureTime =
                       
                };

                return schedule;
            }),

        var newOrUpdatedTrip = new Trip()
        {
            Id = tu.Trip.TripId,
            StopSchedules = tu.StopTimeUpdate.ToList().ConvertAll(stopTimeU =>
            {
                if (_stops!.ContainsKey(stopTimeU.StopId) is false)
                    AddStop(new Stop() { Id = stopTimeU.StopId, Position = new PositionLL() });

                var schedule = (IStopSchedule)new IndexedStopSchedule()
                {
                    StopId = _stops[stopTimeU.StopId],
                    DepartureTime =
                        (DateTime.UnixEpoch.AddSeconds(stopTimeU.Departure?.Time ?? stopTimeU.Arrival?.Time ?? 0L)).AddHours(TimezoneHelper.UtcDiff)
                };

                return schedule;
            }),
        };

        if (_trips!.TryGetValue(newOrUpdatedTrip.Id, out var matchingTrip))
        {
            for (var i = 0; i < matchingTrip.StopSchedules.Count; i++)
            {
                if (matchingTrip.StopSchedules[i].Stop.Id.Equals(newOrUpdatedTrip.Id))
                {
                    matchingTrip.StopSchedules.RemoveRange(i, matchingTrip.StopSchedules.Count - i - 1);
                    matchingTrip.StopSchedules.AddRange(matchingTrip.StopSchedules);

                    break;
                }
            }
        }
        else
            _trips!.AddOrUpdate(newOrUpdatedTrip.Id, (_) => newOrUpdatedTrip, (_, _) => newOrUpdatedTrip);
    }
}