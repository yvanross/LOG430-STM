using Application.Commands.Seedwork;
using Application.CommandServices;
using Application.CommandServices.Repositories;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Domain.Aggregates.Trip;
using Domain.Common.Exceptions;
using Domain.Services.Aggregates;
using Domain.Services.Utility;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;
using static STM.ExternalServiceProvider.Proto.TripUpdate.Types;

namespace Application.Commands.Handlers;

public class UpdateTripsHandler : ICommandHandler<UpdateTrips>
{
    private readonly IStmClient _stmClient;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTripsHandler> _logger;
    private readonly TripServices _tripServices;
    private readonly TimeServices _timeServices;
    private readonly IPublisher _publisher;

    public UpdateTripsHandler(
        IStmClient stmClient,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTripsHandler> logger,
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

    public async Task Handle(UpdateTrips command, CancellationToken cancellation)
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
        tripUpdates = tripUpdates.ToList();

        var updatesIds = tripUpdates.Select(tripUpdate => tripUpdate.Trip.TripId).ToArray();

        var storedTrips = (await _tripRepository.GetAllAsync(updatesIds)).ToDictionary(trip => trip.Id);

        foreach (var tripUpdate in tripUpdates)
        {
            List<StopTimeUpdate> stopTimeUpdates = tripUpdate.StopTimeUpdate.ToList();

            if (storedTrips.TryGetValue(tripUpdate.Trip.TripId, out var storedValue))
            {
                UpdateScheduledStops(storedValue, stopTimeUpdates);
            }
            else
            {
                _logger.LogInformation("Trip not found, creating new trip");

                var trip = CreateTrip(tripUpdate, stopTimeUpdates);

                await _tripRepository.AddAsync(trip);
            }
        }
    }

    private void UpdateScheduledStops(Trip trip, List<StopTimeUpdate> stopTimeUpdates)
    {
        try
        {
            foreach (var stopTimeUpdate in stopTimeUpdates)
            {
                var datetime = GetUpdatedStopScheduledTime(stopTimeUpdate);

                trip.UpdateScheduledStop(stopTimeUpdate.StopId, datetime);
            }
           
        }
        catch (ScheduledStopNotFoundException e)
        {
            _logger.LogInformation(e, "Scheduled stop not found, creating new scheduled stops");

            trip.UpdateScheduledStops(stopTimeUpdates.Select(update => (update.StopId, GetUpdatedStopScheduledTime(update))));

            _tripRepository.Update(trip);
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