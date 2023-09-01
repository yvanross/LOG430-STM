using Domain.Aggregates.Ride;
using Domain.Aggregates.Trip;
using Domain.Factories;

namespace Domain.Services.Aggregates;

public class RideServices
{
    public Ride CreateRide(string busId, Trip trip, string departureId, string destinationId)
    {
        return RideFactory.Create(busId, departureId, destinationId);
    }
}