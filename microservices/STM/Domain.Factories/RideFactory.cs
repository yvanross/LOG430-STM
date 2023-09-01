using Domain.Aggregates.Ride;

namespace Domain.Factories;

internal static class RideFactory
{
    internal static Ride Create(string busId, string departureId, string destinationId)
    {
        var rideId = Guid.NewGuid().ToString();

        var ride = new Ride(rideId, busId, departureId, destinationId);

        ride.TripBegunTime = DateTime.UtcNow;

        return ride;
    }
}