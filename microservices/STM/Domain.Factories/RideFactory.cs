using Domain.Aggregates;
using Domain.Aggregates.Ride;
using Domain.Common.Exceptions;
using Domain.Entities;

namespace Domain.Factories;

internal static class RideFactory
{
    internal static Ride Create(string busId, Trip trip, string departureId, string destinationId)
    {
        var rideId = Guid.NewGuid().ToString();

        var sourceAndDestination = GetOriginAndDestinationFromTrip(trip, departureId, destinationId);

        return new Ride(rideId, busId, sourceAndDestination.Origin, sourceAndDestination.Destination);
    }

    private static (ScheduledStop Origin, ScheduledStop Destination) GetOriginAndDestinationFromTrip(Trip trip, string departureId, string destinationId)
    {
        ScheduledStop? origin = null, destination = null;

        for (var index = 0; index < trip.NumberOfStops(); index++)
        {
            var stopSchedule = trip.GetStopByIndex(index);

            if (origin is null && stopSchedule.StopId.Equals(departureId))
            {
                origin = stopSchedule;
            }
            else if (origin is not null && destination is null && stopSchedule.StopId.Equals(destinationId))
            {
                destination = stopSchedule;
            }
        }

        if (origin is null || destination is null)
            throw new AggregateInvalidStateException("Departure or destination could not be found in Trip for Ride");

        return (origin!, destination!);
    }
}