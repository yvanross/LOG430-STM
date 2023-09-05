using Application.Commands.Seedwork;
using Application.CommandServices;
using Application.CommandServices.Repositories;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Domain.Common.Interfaces;
using Domain.Services.Aggregates;
using Microsoft.Extensions.Logging;

namespace Application.Commands.UpdateBus;

public class UpdateBusesHandler : ICommandHandler<UpdateBusesCommand>
{
    private readonly IBusWriteRepository _busRepository;
    private readonly BusServices _busServices;
    private readonly ILogger<UpdateBusesHandler> _logger;
    private readonly IPublisher _publisher;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly IStmClient _stmClient;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusesHandler(
        IStmClient stmClient,
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateBusesHandler> logger,
        BusServices busServices,
        IPublisher publisher,
        IDatetimeProvider datetimeProvider)
    {
        _stmClient = stmClient;
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _busServices = busServices;
        _publisher = publisher;
        _datetimeProvider = datetimeProvider;
    }

    public async Task Handle(UpdateBusesCommand command, CancellationToken cancellation)
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

                await _busRepository.AddOrUpdateAsync(bus);
            }

            await _unitOfWork.SaveChangesAsync();

            await _publisher.Publish(new BusPositionsUpdated(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating buses");
        }
    }
}