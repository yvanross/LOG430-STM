using Domain.Aggregates;
using Domain.Aggregates.Ride;
using Domain.Common.Interfaces;
using Domain.Factories;

namespace Domain.Services.Aggregates;

public class RideServices
{
    private readonly IDatetimeProvider _datetimeProvider;

    public RideServices(IDatetimeProvider datetimeProvider)
    {
        _datetimeProvider = datetimeProvider;
    }

    public Ride CreateRide(string busId, Trip trip, string departureId, string destinationId)
    {
        return RideFactory.Create(busId, trip, departureId, destinationId, _datetimeProvider);
    }
}