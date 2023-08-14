using Application.CommandServices;
using Application.Common.AntiCorruption;
using Domain.Services;
using System.ComponentModel.Design;

namespace Application.Commands.Handlers;

public class TrackBusHandler : ICommandHandler<TrackBus>
{
    private readonly ApplicationBusServices _busServices;
    private readonly ApplicationTripServices _tripServices;
    private readonly ApplicationRideServices _rideServices;
    private readonly RideServices _domainRideServices;

    public TrackBusHandler(
        ApplicationBusServices busServices, 
        ApplicationTripServices tripServices,
        ApplicationRideServices rideServices,
        RideServices domainRideServices)
    {
        _busServices = busServices;
        _tripServices = tripServices;
        _rideServices = rideServices;
        _domainRideServices = domainRideServices;
    }

    public Task Handle(TrackBus command)
    {
        var trip = _tripServices.Get(command.TripId);

        var ride = _domainRideServices.CreateRide(command.BusId, trip, command.ScheduledDepartureId, command.ScheduledDestinationId);

        _rideServices.Save(ride);
    }
}