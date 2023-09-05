using Application.Commands.Seedwork;
using Application.CommandServices.Repositories;
using Domain.Services.Aggregates;
using Microsoft.Extensions.Logging;

namespace Application.Commands.UpdateRidesTracking;

public class UpdateRidesTrackingHandler : ICommandHandler<UpdateRidesTrackingCommand>
{
    private readonly IBusWriteRepository _busRepository;
    private readonly ILogger<UpdateRidesTrackingHandler> _logger;
    private readonly IRideWriteRepository _rideRepository;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RideServices _rideServices;

    public UpdateRidesTrackingHandler(
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        RideServices rideServices,
        ILogger<UpdateRidesTrackingHandler> logger)
    {
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _rideServices = rideServices;
        _logger = logger;
    }

    public async Task Handle(UpdateRidesTrackingCommand command, CancellationToken cancellation)
    {
        try
        {
            var rides = await _rideRepository.GetAllAsync();

            foreach (var ride in rides)
            {
                var bus = _busRepository.GetAsync(ride.BusId).Result;

                var trip = _tripRepository.GetAsync(bus.TripId).Result;

                _rideServices.UpdateRide(ride, bus, trip);

                if (ride.TrackingComplete)
                {
                    _rideRepository.Remove(ride);
                }
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