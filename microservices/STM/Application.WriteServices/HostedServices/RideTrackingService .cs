using Application.CommandServices.ServiceInterfaces.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices;

public class RideTrackingService : BackgroundService
{
    private readonly IRideWriteRepository _rideRepository;
    private readonly IBusWriteRepository _busRepository;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RideTrackingService> _logger;

    public RideTrackingService(
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<RideTrackingService> logger)
    {
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
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
                        trip.GetIndexOfStop(ride.Destination.StopId));
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating rides");
            }
            
            await Task.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);
        }
    }
}