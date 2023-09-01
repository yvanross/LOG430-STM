using Application.Commands.Seedwork;
using Application.CommandServices.Repositories;
using Domain.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class UpdateRideTrackingHandler : ICommandHandler<UpdateRideTracking>
{
    private readonly IRideWriteRepository _rideRepository;
    private readonly IBusWriteRepository _busRepository;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRideTrackingHandler> _logger;
    private readonly IDatetimeProvider _datetimeProvider;

    public UpdateRideTrackingHandler(
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateRideTrackingHandler> logger,
        IDatetimeProvider datetimeProvider)
    {
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _datetimeProvider = datetimeProvider;
    }

    public async Task Handle(UpdateRideTracking command, CancellationToken cancellation)
    {
        try
        {
            var rides = await _rideRepository.GetAllAsync();

            foreach (var ride in rides)
            {
                var bus = _busRepository.GetAsync(ride.BusId).Result;

                var trip = _tripRepository.GetAsync(bus.TripId).Result;

                var previousStopId = trip.GetStopIdByIndex(bus.CurrentStopIndex);

                ride.UpdateRide(
                    previousStopId, bus.CurrentStopIndex,
                    trip.GetIndexOfStop(ride.DepartureId),
                    trip.GetIndexOfStop(ride.DestinationId),
                    _datetimeProvider);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (ArgumentOutOfRangeException e)
        {
            _logger.LogError(e, "Error while updating rides, index was out of range, ");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating rides");
        }
    }
}