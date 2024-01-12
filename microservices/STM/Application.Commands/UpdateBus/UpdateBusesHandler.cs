using Application.Commands.Seedwork;
using Application.CommandServices.Interfaces;
using Application.CommandServices.Repositories;
using Application.EventHandlers.Interfaces;
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
    private readonly IEventPublisher _eventPublisher;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly IStmClient _stmClient;
    private readonly IUnitOfWork _unitOfWork;

    private const int MaxBusAgeInMinutes = 30;

    public UpdateBusesHandler(
        IStmClient stmClient,
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateBusesHandler> logger,
        BusServices busServices,
        IEventPublisher eventPublisher,
        IDatetimeProvider datetimeProvider)
    {
        _stmClient = stmClient;
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _busServices = busServices;
        _eventPublisher = eventPublisher;
        _datetimeProvider = datetimeProvider;
    }

    public async Task Handle(UpdateBusesCommand command, CancellationToken cancellation)
    {
        try
        {
            var feedPositions = _stmClient.RequestFeedPositions();

            _busRepository.RemoveOldBuses(GetMaxBusAge());

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

            await _eventPublisher.Publish(new BusPositionsUpdated(Guid.NewGuid(), _datetimeProvider.GetCurrentTime()));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating buses");
        }
    }

    private DateTime GetMaxBusAge()
    {
        return _datetimeProvider.GetCurrentTime().Subtract(TimeSpan.FromMinutes(MaxBusAgeInMinutes));
    }
}