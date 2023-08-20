using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.QueryServices.ServiceInterfaces.Repositories;
using Application.ViewModels;
using Domain.Aggregates;
using Microsoft.Extensions.Logging;

namespace Application.QueryServices;

public class ApplicationBusServices
{
    private readonly IBusReadRepository _busRead;
    private readonly ILogger<ApplicationBusServices> _logger;

    public ApplicationBusServices(IBusReadRepository busRead, ILogger<ApplicationBusServices> logger)
    {
        _busRead = busRead;
        _logger = logger;
    }

    public IEnumerable<RideViewModel> GetTimeRelevantRideViewModels(Dictionary<string, Trip> trips, Dictionary<string, Stop> sources, Dictionary<string, Stop> destination)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting time relevant ride view models");
            throw;
        }
    }
}