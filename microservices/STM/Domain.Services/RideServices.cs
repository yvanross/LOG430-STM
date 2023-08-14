using Domain.Aggregates;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Entities;
using Domain.Factories;

namespace Domain.Services;

public class RideServices
{
    private readonly IDatetimeProvider _datetimeProvider;

    public RideServices(IDatetimeProvider datetimeProvider)
    {
        _datetimeProvider = datetimeProvider;
    }

    public Ride CreateRide(string busId, Trip trip, string sourceId, string destinationId)
    {
        var rideId = Guid.NewGuid().ToString();

        var sourceAndDestination = GetOriginAndDestinationFromTrip(trip, sourceId, destinationId);

        return new Ride(rideId, busId, sourceAndDestination.Origin, sourceAndDestination.Destination);
    }

    public IEnumerable<Ride> GetRidesFromPositions(IEnumerable<(Bus busDto, Trip Trip)> busAndTrips)
    {

    }

    public IEnumerable<Ride> GetTimeRelevantRides(IEnumerable<Ride> rides)
    {
        return rides.Where(ride => ride.IsTimeRelevant(_datetimeProvider)).ToList();
    }

    private (ScheduledStop Origin, ScheduledStop Destination) GetOriginAndDestinationFromTrip(Trip trip, string sourceId, string destinationId)
    {
        ScheduledStop? origin = null, destination = null;

        for (var index = 0; index < trip.NumberOfStops(); index++)
        {
            var stopSchedule = trip.GetStopByIndex(index);

            if (origin is null && stopSchedule.StopId.Equals(sourceId))
            {
                origin = stopSchedule;
            }
            else if (destination is not null && Destination is null && stopSchedule.StopId.Equals(destinationId))
            {
                destination = stopSchedule;
            }
        }

        if (origin is null || destination is null)
            throw new AggregateInvalidStateException("Departure or destination could not be found in Trip for Ride");

        return (origin!, destination!);
    }
}