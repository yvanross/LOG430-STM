using Application.Commands.Seedwork;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Microsoft.Extensions.Logging;
using Domain.Common.Exceptions;
using Domain.Services.Aggregates;

namespace Application.Commands.Handlers;

public class TrackBusHandler : ICommandHandler<TrackBus>
{
    private readonly ITripWriteRepository _tripRepository;
    private readonly IRideWriteRepository _rideRepository;
    private readonly IBusWriteRepository _busRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RideServices _domainRideServices;
    private readonly ILogger<TrackBusHandler> _logger;

    public TrackBusHandler(
        ITripWriteRepository tripRepository,
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        RideServices domainRideServices,
        ILogger<TrackBusHandler> logger)
    {
        _tripRepository = tripRepository;
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
        _domainRideServices = domainRideServices;
        _logger = logger;
    }

    public async Task Handle(TrackBus command, CancellationToken cancellation)
    {
        try
        {
            var bus = await _busRepository.GetAsync(command.BusId);

            var trip = await _tripRepository.GetAsync(bus.TripId);

            var ride = _domainRideServices.CreateRide(command.BusId, trip, command.ScheduledDepartureId, command.ScheduledDestinationId);

            await _rideRepository.AddAsync(ride);

            await _unitOfWork.SaveChangesAsync();
        }
        catch (KeyNotFoundException e)
        {
            throw;
        }
        catch (AggregateInvalidStateException e)
        {
            _logger.LogError($"An error occurred while creating ride. Exception: {e.Message}");

            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"An unknown error occurred while tracking bus with ID {command.BusId}. Exception: {e.Message}");

            throw;
        }
    }
}