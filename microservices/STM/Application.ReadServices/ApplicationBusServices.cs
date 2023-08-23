using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.QueryServices.ServiceInterfaces;
using Application.ViewModels;
using Domain.Aggregates.Bus;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;

namespace Application.QueryServices;

public class ApplicationBusServices
{
    private readonly IQueryContext _busRead;
    private readonly ILogger<ApplicationBusServices> _logger;

    public ApplicationBusServices(IQueryContext busRead, ILogger<ApplicationBusServices> logger)
    {
        _busRead = busRead;
        _logger = logger;
    }

    public IEnumerable<RideViewModel> GetTimeRelevantRideViewModels(Dictionary<string, Trip> trips, HashSet<string> sources, HashSet<string> destination)
    {
        try
        {
            var materializedTrips = trips.Keys.ToList();

            var buses = _busRead.GetData<Bus>().Where(bus => materializedTrips.Contains(bus.TripId));

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
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting time relevant ride view models");
            throw;
        }
    }
}