using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.QueryServices.ServiceInterfaces.Repositories;
using Application.ViewModels;
using Domain.Aggregates;

namespace Application.QueryServices;

public class ApplicationBusServices
{
    private readonly IBusReadRepository _busRead;

    public ApplicationBusServices(IBusReadRepository busRead)
    {
        _busRead = busRead;
    }

    public IEnumerable<RideViewModel> GetTimeRelevantRideViewModels(Dictionary<string, Trip> trips, Dictionary<string, Stop> sources, Dictionary<string, Stop> destination)
    {
        var buses = _busRead.GetAllIdsMatchingTripsIds(trips.Keys);

        var rideViewModels = (
            from bus in buses
            let tripId = bus.TripId
            let ride = new RideViewModel(
                trips[bus.TripId].FirstMatchingStop(sources).Id,
                trips[bus.TripId].LastMatchingStop(destination).Id,
                bus.Id)
            orderby trips[tripId].GetStopDepartureTime(ride.ScheduledDepartureId)
            select ride).ToList();

        if (rideViewModels.IsEmpty()) throw new NoBusesFoundException();

        return rideViewModels;
    }
}