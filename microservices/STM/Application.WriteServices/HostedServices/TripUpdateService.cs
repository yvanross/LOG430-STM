using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Domain.Services.Aggregates;
using Domain.Services.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;
using static STM.ExternalServiceProvider.Proto.TripUpdate.Types;

namespace Application.CommandServices.HostedServices;

public class TripUpdateService : BackgroundService
{
    private readonly IStmClient _stmClient;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TripUpdateService> _logger;
    private readonly TripServices _tripServices;
    private readonly TimeServices _timeServices;

    public TripUpdateService(
        IStmClient stmClient, 
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<TripUpdateService> logger,
        TripServices tripServices,
        TimeServices timeServices)
    {
        _stmClient = stmClient;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _tripServices = tripServices;
        _timeServices = timeServices;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tripUpdates = await _stmClient.RequestFeedTripUpdates();

                await ProcessTripUpdates(tripUpdates);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating trips");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessTripUpdates(IEnumerable<TripUpdate> tripUpdates)
    {
        foreach (var tripUpdate in tripUpdates)
        {
            List<StopTimeUpdate> stopTimeUpdates = tripUpdate.StopTimeUpdate.ToList();

            Trip trip;

            try
            {
                trip = await _tripRepository.GetAsync(tripUpdate.Trip.TripId);

                UpdateScheduledStops(trip, stopTimeUpdates);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogInformation(e, "Trip not found, creating new trip");

                trip = CreateTrip(tripUpdate, stopTimeUpdates);

                await _tripRepository.AddAsync(trip);
            }
        }
    }

    private void UpdateScheduledStops(Trip trip, List<StopTimeUpdate> stopTimeUpdates)
    {
        foreach (var stopTimeUpdate in stopTimeUpdates)
        {
            var datetime = GetUpdatedStopScheduledTime(stopTimeUpdate);

            trip.UpdateScheduledStops(stopTimeUpdate.StopId, datetime);
        }
    }


    private Trip CreateTrip(TripUpdate tripUpdate, List<StopTimeUpdate> stopTimeUpdates)
    {
        var updatedStopTimes = stopTimeUpdates
            .ConvertAll(stopTimeUpdate => (stopTimeUpdate.StopId, GetUpdatedStopScheduledTime(stopTimeUpdate)));

        var trip = _tripServices.CreateTrip(tripUpdate.Trip.TripId, updatedStopTimes);

        return trip;
    }

    private DateTime GetUpdatedStopScheduledTime(StopTimeUpdate stopTimeUpdate)
    {
        var datetime = _timeServices.LongToDatetime(stopTimeUpdate.Departure?.Time ?? stopTimeUpdate.Arrival?.Time ?? 0L);

        return datetime;
    }
}