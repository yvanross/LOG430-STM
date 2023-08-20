using Application.CommandServices.HostedServices.Workers;
using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Domain.Services.Aggregates;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices.Processors;

public class BusUpdateProcessor : IScopedProcessor
{
    private readonly IStmClient _stmClient;
    private readonly IBusWriteRepository _busRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BusUpdateService> _logger;
    private readonly BusServices _busServices;
    private readonly IPublisher _publisher;

    public BusUpdateProcessor(
        IStmClient stmClient,
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        ILogger<BusUpdateService> logger,
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

    public async Task ProcessUpdates()
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