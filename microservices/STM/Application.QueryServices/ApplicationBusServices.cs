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

    private List<Bus> GetRelevantBuses(HashSet<string> tripKeys)
    {
        return _busRead.GetData<Bus>().Where(bus => tripKeys.Contains(bus.TripId)).ToList();
    }

    private List<(RideViewModel RideViewModel, TimeSpan firstStopTimespan)> BuildRideViewModels(
        List<Bus> buses,
        Dictionary<string, List<ScheduledStopDto>> tripProjection,
        HashSet<string> sources, 
        HashSet<string> destinations)
    {
        return buses
            .Select(bus => TryBuildRideViewModel(bus, tripProjection[bus.TripId], sources, destinations))
            .Where(rideViewModel => rideViewModel is not null)
            .Select(rideViewModel => rideViewModel!.Value)
            .ToList();
    }

    private (RideViewModel RideViewModel, TimeSpan firstStopTimespan)? TryBuildRideViewModel(
        Bus bus, 
        List<ScheduledStopDto> scheduledStopDtos, 
        HashSet<string> sources,
        HashSet<string> destinations)
    {
        if (TryFindStops(bus, scheduledStopDtos, sources, destinations) is {} stops)
        {
            var rideViewModel = new RideViewModel(stops.FirstStop.StopId, stops.DestinationStop.StopId, bus.Id);

            var firstStopTimespan = CalculateFirstStopTimespan(stops);

            return (rideViewModel, firstStopTimespan);
        }

        return default;
    }

    private TimeSpan CalculateFirstStopTimespan((ScheduledStopDto FirstStop, ScheduledStopDto DestinationStop) stops)
    {
        var numberOfHoursInADayOnEarth = TimeSpan.FromHours(24);

        var currentTimeOfDay = _datetimeProvider.GetCurrentTime().TimeOfDay;

        var possibleTimespans = new List<TimeSpan>() { stops.FirstStop.DepartureTimespan };

        if (stops.FirstStop.DepartureTimespan >= numberOfHoursInADayOnEarth)
            possibleTimespans.Add(TimeSpan.FromSeconds(stops.FirstStop.DepartureTimespan.TotalSeconds %
                                                       numberOfHoursInADayOnEarth.TotalSeconds));

        return possibleTimespans.MinBy(timespan => timespan - currentTimeOfDay);
    }

    private static (ScheduledStopDto FirstStop, ScheduledStopDto DestinationStop)? TryFindStops(
        Bus bus, 
        List<ScheduledStopDto> scheduledStopDtos, 
        HashSet<string> sources, 
        HashSet<string> destinations)
    {
        ScheduledStopDto firstStop, destinationStop;

        scheduledStopDtos = scheduledStopDtos.OrderBy(scheduledStopDto => scheduledStopDto.StopSequence).ToList();

        try
        {
            firstStop = scheduledStopDtos.First(scheduledStopDto => sources.Contains(scheduledStopDto.StopId));
            destinationStop = scheduledStopDtos.Last(scheduledStopDto => destinations.Contains(scheduledStopDto.StopId));

            if (firstStop.StopSequence < destinationStop.StopSequence && bus.CurrentStopIndex < firstStop.StopSequence)
            {
                return (firstStop, destinationStop);
            }
        }
        catch (Exception)
        {
            //pass through, it will return default
        }

        return default;
    }

    private IEnumerable<RideViewModel> SortAndReturnViewModels(List<(RideViewModel RideViewModel, TimeSpan firstStopTimespan)> viewModels)
    {
        var sortedViewModels = viewModels
            .OrderBy(viewModel => viewModel.firstStopTimespan - _datetimeProvider.GetCurrentTime().TimeOfDay)
            .Select(viewModel => viewModel.RideViewModel)
            .ToList();

        if (!sortedViewModels.Any()) throw new NoBusesFoundException();

        return sortedViewModels;
    }
}