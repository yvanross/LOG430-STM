using Application.Mapping.Interfaces;
using Domain.Common.Interfaces;
using Domain.Entities;
using STM.ExternalServiceProvider.Proto;

namespace Application.Mapping;

public class StopMapper : IMappingTo<TripUpdate, ScheduledStop>
{
    private readonly IDatetimeProvider _datetimeProvider;

    public StopMapper(IDatetimeProvider datetimeProvider )
    {
        _datetimeProvider = datetimeProvider;
    }

    public ScheduledStop MapTo(TripUpdate dto)
    {
        dto.
        
        var scheduledStops =  materializedStopTimeUpdates.ConvertAll(stopTimeUpdate => new ScheduledStop(stopTimeUpdate.StopId, (DateTime.UnixEpoch.AddSeconds(stopTimeUpdate.Departure?.Time ?? stopTimeUpdate.Arrival?.Time ?? 0L)).AddHours(_datetimeProvider.GetUtcDifference())));
        return 
    }
}