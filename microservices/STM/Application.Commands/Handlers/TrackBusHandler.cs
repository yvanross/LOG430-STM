using Application.Commands.AntiCorruption;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Microsoft.Extensions.Logging;
using Domain.Common.Exceptions;
using Domain.Services.Aggregates;

namespace Application.Commands.Handlers;

public class TrackBusHandler : ICommandHandler<TrackBus>
{
    private readonly ITripWriteRepository _tripRepository;
    private readonly IRideWriteRepository _rideRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RideServices _domainRideServices;
    private readonly ILogger<TrackBusHandler> _logger;

    public TrackBusHandler(
        ITripWriteRepository tripRepository,
        IRideWriteRepository rideRepository,
        IUnitOfWork unitOfWork,
        RideServices domainRideServices,
        ILogger<TrackBusHandler> logger)
    {
        _tripRepository = tripRepository;
        _rideRepository = rideRepository;
        _unitOfWork = unitOfWork;
        _domainRideServices = domainRideServices;
        _logger = logger;
    }

    public async Task Handle(TrackBus command, CancellationToken cancellation)
    {
        try
        {
            var trip = await _tripRepository.GetAsync(command.TripId);

            var ride = _domainRideServices.CreateRide(command.BusId, trip, command.ScheduledDepartureId, command.ScheduledDestinationId);

            await _rideRepository.AddAsync(ride);

            await _unitOfWork.SaveChangesAsync();
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"Trip with ID {command.TripId} was not found. Exception: {e.Message}");

            throw;
        }
        catch (AggregateInvalidStateException e)
        {
            _logger.LogError($"An invalid state occurred while processing the trip with ID {command.TripId}. Exception: {e.Message}");

            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"An unexpected error occurred while processing the trip with ID {command.TripId}. Exception: {e.Message}");

            throw;
        }
    }
}