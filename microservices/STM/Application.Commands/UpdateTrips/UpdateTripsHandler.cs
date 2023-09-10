using Application.Commands.Seedwork;
using Application.CommandServices.Interfaces;
using Application.CommandServices.Repositories;
using Domain.Aggregates.Trip;
using Domain.Services.Aggregates;
using Domain.Services.Utility;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;
using static STM.ExternalServiceProvider.Proto.TripUpdate.Types;

namespace Application.Commands.UpdateTrips;

public class UpdateTripsHandler : ICommandHandler<UpdateTripsCommand>
{
    private readonly ILogger<UpdateTripsHandler> _logger;
    private readonly IStmClient _stmClient;
    private readonly TimeServices _timeServices;
    private readonly ITripWriteRepository _tripRepository;
    private readonly TripServices _tripServices;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTripsHandler(
        IStmClient stmClient,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTripsHandler> logger,
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

    public async Task Handle(UpdateTripsCommand command, CancellationToken cancellation)
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
    }

    private async Task ProcessTripUpdates(IEnumerable<TripUpdate> tripUpdates)
    {
        foreach (var tripUpdate in tripUpdates)
        {
            List<StopTimeUpdate> stopTimeUpdates = tripUpdate.StopTimeUpdate.ToList();

            switch (tripUpdate.Trip.ScheduleRelationship)
            {
                case TripDescriptor.Types.ScheduleRelationship.Scheduled:
                    //updating scheduled stops time to the minute is not actually worth it since this isn't a production software
                    break;
                case TripDescriptor.Types.ScheduleRelationship.Added:

                    _logger.LogInformation("Creating new trip");

                    var trip = CreateTrip(tripUpdate, stopTimeUpdates);

                    await _tripRepository.AddOrUpdateAsync(trip);

                    break;
                case TripDescriptor.Types.ScheduleRelationship.Unscheduled:
                case TripDescriptor.Types.ScheduleRelationship.Canceled:
                    //not worth db call to handle it
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private Trip CreateTrip(TripUpdate tripUpdate, List<StopTimeUpdate> stopTimeUpdates)
    {
        var newStopTimes = stopTimeUpdates
            .ConvertAll(stopTimeUpdate =>
            {
                var stopId = stopTimeUpdate.StopId;

                var secondsSinceEpoch =
                    stopTimeUpdate.Departure?.Time ??
                    stopTimeUpdate.Arrival?.Time ??
                    throw new Exception("timespan was null will creating a new Trip based on Feed Trip Data");

                var datetime = _timeServices.LongToDatetime(secondsSinceEpoch);

                return (stopId, datetime.TimeOfDay);
            });

        var trip = _tripServices.CreateTrip(tripUpdate.Trip.TripId, newStopTimes);

        return trip;
    }
}