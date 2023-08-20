using Domain.Aggregates.Ride;
using Domain.Aggregates.Trip;
using Domain.Common.Exceptions;

namespace Domain.Factories;

internal static class RideFactory
{
    internal static Ride Create(string busId, Trip trip, string departureId, string destinationId)
    {
        var rideId = Guid.NewGuid().ToString();

        var sourceAndDestination = GetOriginAndDestinationFromTrip(trip, departureId, destinationId);

        return new Ride(rideId, busId, sourceAndDestination.OriginId, sourceAndDestination.DestinationId);
    }

    private static (string OriginId, string DestinationId) GetOriginAndDestinationFromTrip(Trip trip, string departureId, string destinationId)
    {
        string? origin = null, destination = null;

        for (var index = 0; index < trip.NumberOfStops(); index++)
        {
            var stopId = trip.GetStopIdByIndex(index);

            if (origin is null && stopId.Equals(departureId))
            {
                origin = trip.GetScheduleIdByStopId(stopId);
            }
            else if (origin is not null && destination is null && stopId.Equals(destinationId))
            {
                destination = trip.GetScheduleIdByStopId(stopId);
            }
        }

        if (origin is null || destination is null)
            throw new AggregateInvalidStateException("Departure or destination could not be found in Trip for Ride");

        return (origin!, destination!);
    }
}