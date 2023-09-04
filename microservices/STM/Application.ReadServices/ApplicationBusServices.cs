using Application.Common.Exceptions;
using Application.QueryServices.ServiceInterfaces;
using Application.ViewModels;
using Domain.Aggregates.Bus;
using Domain.Aggregates.Trip;
using Domain.Common.Exceptions;
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

    public IEnumerable<RideViewModel> GetTimeRelevantRideViewModels(
        Dictionary<string, Trip> trips,
        HashSet<string> sources,
        HashSet<string> destinations)
    {
        try
        {
            var relevantBuses = GetRelevantBuses(trips.Keys.ToHashSet());

            var viewModels = BuildRideViewModels(relevantBuses, trips, sources, destinations);

            return SortAndReturnViewModels(viewModels, trips);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting time relevant ride view models");
            throw;
        }
    }

    private List<Bus> GetRelevantBuses(HashSet<string> materializedTrips)
    {
        return _busRead.GetData<Bus>().Where(bus => materializedTrips.Contains(bus.TripId)).ToList();
    }

    private List<(RideViewModel RideViewModel, string TripId)> BuildRideViewModels(
        List<Bus> buses,
        Dictionary<string, Trip> trips,
        HashSet<string> sources, 
        HashSet<string> destinations)
    {
        var viewModels = new List<(RideViewModel RideViewModel, string TripId)>();

        foreach (var bus in buses)
        {
            if (TryBuildRideViewModel(bus, trips, sources, destinations, out var rideViewModel, out var tripId))
            {
                viewModels.Add((rideViewModel, tripId));
            }
        }

        return viewModels;
    }

    private bool TryBuildRideViewModel(
        Bus bus, 
        Dictionary<string, Trip> trips, 
        HashSet<string> sources,
        HashSet<string> destinations,
        out RideViewModel rideViewModel,
        out string tripId)
    {
        rideViewModel = default;
        tripId = string.Empty;

        string firstStopId, destinationStopId;

        var trip = trips[bus.TripId];

        try
        {
            firstStopId = trip.FirstMatchingStop(sources).StopId;
            destinationStopId = trip.LastMatchingStop(destinations).StopId;
        }
        catch (ScheduledStopNotFoundException)
        {
            return false;
        }

        var indexOfFirstStopOnTrip = trip.GetIndexOfStop(firstStopId);

        if (trip.IsDepartureAndDestinationInRightOrder(firstStopId, destinationStopId) is false ||
            bus.CurrentStopIndex >= indexOfFirstStopOnTrip) 
            return false;
        
        rideViewModel = new RideViewModel(firstStopId, destinationStopId, bus.Id);

        tripId = bus.TripId;

        return true;

    }

    private IEnumerable<RideViewModel> SortAndReturnViewModels(
        List<(RideViewModel RideViewModel, string TripId)> viewModels, Dictionary<string, Trip> trips)
    {
        var sortedViewModels = viewModels
            .OrderBy(viewModel =>
                trips[viewModel.TripId].GetStopDepartureTime(viewModel.RideViewModel.ScheduledDepartureId))
            .Select(viewModel => viewModel.RideViewModel)
            .ToList();

        if (!sortedViewModels.Any()) throw new NoBusesFoundException();

        return sortedViewModels;
    }
}