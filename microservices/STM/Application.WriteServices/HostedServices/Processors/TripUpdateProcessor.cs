using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Domain.Aggregates;
using Domain.Services.Aggregates;
using Domain.Services.Utility;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;
using static STM.ExternalServiceProvider.Proto.TripUpdate.Types;

namespace Application.CommandServices.HostedServices.Processors;

public class TripUpdateProcessor : IScopedProcessor
{
    private readonly IStmClient _stmClient;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TripUpdateProcessor> _logger;
    private readonly TripServices _tripServices;
    private readonly TimeServices _timeServices;
    private readonly IPublisher _publisher;

    public TripUpdateProcessor(
        IStmClient stmClient,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<TripUpdateProcessor> logger,
        TripServices tripServices,
        TimeServices timeServices,
        IPublisher publisher)
    {
        _stmClient = stmClient;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _tripServices = tripServices;
        _timeServices = timeServices;
        _publisher = publisher;
    }

    public async Task ProcessUpdates()
    {
        try
        {
            var tripUpdates = await _stmClient.RequestFeedTripUpdates();

            await ProcessTripUpdates(tripUpdates);

            await _unitOfWork.SaveChangesAsync();

            await _publisher.Publish(new StmTripModificationApplied());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating trips");
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