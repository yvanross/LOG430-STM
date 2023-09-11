using Domain.Aggregates.Bus;
using Domain.Aggregates.Ride;
using Domain.Aggregates.Trip;
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

    public Ride CreateRide(Bus bus, Trip trip, string departureId, string destinationId)
    {
        return RideFactory.Create(bus.Id, trip.GetStopIdByIndex(bus.CurrentStopIndex), departureId, destinationId, _datetimeProvider);
    }

    public void UpdateRide(Ride ride, Bus bus, Trip trip)
    {
        trip.ValidateStopIndex(bus.CurrentStopIndex);

        ride.UpdateRide(
            new RideUpdateInfo(
                trip.GetIndexOfStop(ride.FirstRecordedStopId),
                bus.CurrentStopIndex,
                trip.GetIndexOfStop(ride.DepartureId),
                trip.GetIndexOfStop(ride.DestinationId),
                bus.Name), 
            _datetimeProvider);
    }

    public void CompleteTracking(Ride ride)
    {
        ride.CompleteTracking(_datetimeProvider);
    }
}