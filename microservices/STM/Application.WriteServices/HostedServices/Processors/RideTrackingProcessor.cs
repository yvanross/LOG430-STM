using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices.Processors;

public class RideTrackingProcessor : IScopedProcessor
{
    private readonly IRideWriteRepository _rideRepository;
    private readonly IBusWriteRepository _busRepository;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RideTrackingProcessor> _logger;
    private readonly IDatetimeProvider _datetimeProvider;

    public RideTrackingProcessor(
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<RideTrackingProcessor> logger,
        IDatetimeProvider datetimeProvider)
    {
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _datetimeProvider = datetimeProvider;
    }

    public async Task ProcessUpdates()
    {
        try
        {
            var rides = await _rideRepository.GetAllAsync();

            foreach (var ride in rides)
            {
                var bus = _busRepository.GetAsync(ride.BusId).Result;

                var trip = _tripRepository.GetAsync(bus.TripId).Result;

                var previousStop = trip.GetStopByIndex(bus.CurrentStopIndex);

                ride.UpdateRide(
                    previousStop, bus.CurrentStopIndex,
                    trip.GetIndexOfStop(ride.Departure.StopId),
                    trip.GetIndexOfStop(ride.Destination.StopId),
                    _datetimeProvider);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating rides");
        }
    }
}