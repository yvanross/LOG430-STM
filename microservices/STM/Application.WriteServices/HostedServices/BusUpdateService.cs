using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Services.Aggregates;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices;

public class BusUpdateService : BackgroundService
{
    private readonly IStmClient _stmClient;
    private readonly IBusWriteRepository _busRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BusUpdateService> _logger;
    private readonly BusServices _busServices;

    public BusUpdateService(
        IStmClient stmClient, 
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        ILogger<BusUpdateService> logger,
        BusServices busServices)
    {
        _stmClient = stmClient;
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _busServices = busServices;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
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
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating buses");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}