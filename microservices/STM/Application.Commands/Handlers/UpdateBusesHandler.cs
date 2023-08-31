using Application.Commands.Seedwork;
using Application.CommandServices;
using Application.CommandServices.Repositories;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Domain.Services.Aggregates;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class UpdateBusesHandler : ICommandHandler<UpdateBuses>
{
    private readonly IStmClient _stmClient;
    private readonly IBusWriteRepository _busRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateBusesHandler> _logger;
    private readonly BusServices _busServices;
    private readonly IPublisher _publisher;

    public UpdateBusesHandler(
        IStmClient stmClient,
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateBusesHandler> logger,
        BusServices busServices,
        IPublisher publisher)
    {
        _stmClient = stmClient;
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _busServices = busServices;
        _publisher = publisher;
    }

    public async Task Handle(UpdateBuses command, CancellationToken cancellation)
    {
        try
        {
            var feedPositions = _stmClient.RequestFeedPositions();

            foreach (var feedPosition in feedPositions)
            {
                var bus = _busServices.CreateBus(
                    feedPosition.Vehicle.Id,
                    feedPosition.Trip.RouteId,
                    feedPosition.Trip.TripId,
                    Convert.ToInt32(feedPosition.CurrentStopSequence));

                await _busRepository.AddAsync(bus);
            }

            await _unitOfWork.SaveChangesAsync();

            await _publisher.Publish(new BusPositionsUpdated());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating buses");
        }
    }
}