using Domain.Aggregates;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Factories;

internal class RideFactory
{
    internal static Ride CreateRide(
        string busId,
        string busName,
        Position busPosition,
        int currentStopIndex, 
        Trip trip,
        ScheduledStop source,
        ScheduledStop destination)
    {
        var bus = new Bus(busId, busName, busPosition);

        return new Ride(Guid.NewGuid().ToString(), bus, );
    }
}