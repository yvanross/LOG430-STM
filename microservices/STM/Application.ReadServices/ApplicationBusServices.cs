using Application.Common.Exceptions;
using Application.Common.Extensions;
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

    public IEnumerable<RideViewModel> GetTimeRelevantRideViewModels(Dictionary<string, Trip> trips, HashSet<string> sources, HashSet<string> destination)
    {
        try
        {
            var materializedTrips = trips.Keys.ToList();

            var buses = _busRead.GetData<Bus>().Where(bus => materializedTrips.Contains(bus.TripId)).ToList();

            List<(RideViewModel RideViewModel, string TripId)> viewModels = new ();

            foreach (var bus in buses)
            {
                RideViewModel? rideViewModel;

                try
                {
                    var firstStopId = trips[bus.TripId].FirstMatchingStop(sources).StopId;
                    var destinationStopId = trips[bus.TripId].LastMatchingStop(destination).StopId;

                    if(trips[bus.TripId].IsDepartureAndDestinationInRightOrder(firstStopId, destinationStopId) is false)
                        continue;

                    rideViewModel = new RideViewModel(firstStopId, destinationStopId, bus.Id);
                }
                catch (ScheduledStopNotFoundException)
                {
                    continue;
                }

                viewModels.Add((rideViewModel.Value, bus.TripId));
            }

            var rideViewModels = viewModels
                .OrderBy(viewModel => trips[viewModel.TripId].GetStopDepartureTime(viewModel.RideViewModel.ScheduledDepartureId))
                .Select(viewModel => viewModel.RideViewModel)
                .ToList();

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