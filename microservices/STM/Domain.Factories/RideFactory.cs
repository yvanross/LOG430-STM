using Domain.Aggregates.Ride;
using Domain.Common.Interfaces;

namespace Domain.Factories;

internal static class RideFactory
{
    internal static Ride Create(string busId, string firstRecordedStopId, string departureId, string destinationId, IDatetimeProvider datetimeProvider)
    {
        var rideId = Guid.NewGuid().ToString();

        var ride = new Ride(rideId, busId, firstRecordedStopId, departureId, destinationId);

        ride.SetTripBegunTime(datetimeProvider.GetCurrentTime());

        return ride;
    }
}