using Application.Common.Exceptions;
using Application.Dtos;
using Application.QueryServices.ServiceInterfaces;
using Application.ViewModels;
using Domain.Aggregates.Bus;
using Domain.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.QueryServices;

public class ApplicationBusServices
{
    private readonly IQueryContext _busRead;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ILogger<ApplicationBusServices> _logger;

    public ApplicationBusServices(IQueryContext busRead, IDatetimeProvider datetimeProvider, ILogger<ApplicationBusServices> logger)
    {
        _busRead = busRead;
        _datetimeProvider = datetimeProvider;
        _logger = logger;
    }

    public IEnumerable<RideViewModel> GetTimeRelevantRideViewModels(
        Dictionary<string, List<ScheduledStopDto>> tripProjection,
        HashSet<string> sources,
        HashSet<string> destinations)
    {
        try
        {
            var relevantBuses = GetRelevantBuses(new HashSet<string>(tripProjection.Keys));

            var viewModels = BuildRideViewModels(relevantBuses, tripProjection, sources, destinations);

            return SortAndReturnViewModels(viewModels);
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

    private List<(RideViewModel RideViewModel, TimeSpan firstStopTimespan)> BuildRideViewModels(
        List<Bus> buses,
        Dictionary<string, List<ScheduledStopDto>> tripProjection,
        HashSet<string> sources, 
        HashSet<string> destinations)
    {
        var viewModels = new List<(RideViewModel RideViewModel, TimeSpan firstStopTimespan)>();

        foreach (var bus in buses)
        {
            if (TryBuildRideViewModel(bus, tripProjection[bus.TripId], sources, destinations, out var rideViewModel, out var firstStopTimespan))
            {
                viewModels.Add((rideViewModel, firstStopTimespan));
            }
        }

        return viewModels;
    }

    private bool TryBuildRideViewModel(
        Bus bus, 
        List<ScheduledStopDto> scheduledStopDtos, 
        HashSet<string> sources,
        HashSet<string> destinations,
        out RideViewModel rideViewModel,
        out TimeSpan firstStopTimespan)
    {
        rideViewModel = default;
        firstStopTimespan = default;

        ScheduledStopDto firstStop, destinationStop;

        scheduledStopDtos = scheduledStopDtos.OrderBy(scheduledStopDto => scheduledStopDto.StopSequence).ToList();

        try
        {
            firstStop = scheduledStopDtos.First(scheduledStopDto => sources.Contains(scheduledStopDto.StopId));
            destinationStop = scheduledStopDtos.Last(scheduledStopDto => destinations.Contains(scheduledStopDto.StopId));
        }
        catch (Exception)
        {
            return false;
        }

        if (firstStop.StopSequence < destinationStop.StopSequence is false || bus.CurrentStopIndex >= firstStop.StopSequence) 
            return false;
        
        rideViewModel = new RideViewModel(firstStop.StopId, destinationStop.StopId, bus.Id);

        var numberOfHoursInADayOnEarth = TimeSpan.FromHours(24);

        var currentTimeOfDay = _datetimeProvider.GetCurrentTime().TimeOfDay;

        var possibleTimespans = new List<TimeSpan>() { firstStop.DepartureTimespan };

        if (firstStop.DepartureTimespan >= numberOfHoursInADayOnEarth)
            possibleTimespans.Add(TimeSpan.FromSeconds(firstStop.DepartureTimespan.TotalSeconds % numberOfHoursInADayOnEarth.TotalSeconds));

        firstStopTimespan = possibleTimespans.MinBy(timespan => timespan - currentTimeOfDay);

        return true;

    }

    private IEnumerable<RideViewModel> SortAndReturnViewModels(
        List<(RideViewModel RideViewModel, TimeSpan firstStopTimespan)> viewModels)
    {
        var sortedViewModels = viewModels
            .OrderBy(viewModel => viewModel.firstStopTimespan - _datetimeProvider.GetCurrentTime().TimeOfDay)
            .Select(viewModel => viewModel.RideViewModel)
            .ToList();

        if (!sortedViewModels.Any()) throw new NoBusesFoundException();

        return sortedViewModels;
    }
}