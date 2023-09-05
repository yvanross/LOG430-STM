using Application.Commands.Seedwork;
using Application.CommandServices;
using Application.CommandServices.Repositories;
using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;

namespace Application.Commands.LoadStaticGtfs;

public class LoadStaticGtfsHandler : ICommandHandler<LoadStaticGtfsCommand>
{
    private readonly ILogger<LoadStaticGtfsHandler> _logger;
    private readonly IMappingTo<IStopWrapper, Stop> _stopMapper;
    private readonly IStopWriteRepository _stopWriteRepository;
    private readonly ITransitDataReader _transitDataReader;
    private readonly IMappingTo<ITripWrapper, Trip> _tripMapper;
    private readonly ITripWriteRepository _tripWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoadStaticGtfsHandler(
        ITripWriteRepository tripWriteRepository,
        IStopWriteRepository stopWriteRepository,
        ITransitDataReader transitDataReader,
        IUnitOfWork unitOfWork,
        IMappingTo<ITripWrapper, Trip> tripMapper,
        IMappingTo<IStopWrapper, Stop> stopMapper,
        ILogger<LoadStaticGtfsHandler> logger)
    {
        _tripWriteRepository = tripWriteRepository;
        _stopWriteRepository = stopWriteRepository;
        _transitDataReader = transitDataReader;
        _unitOfWork = unitOfWork;
        _tripMapper = tripMapper;
        _stopMapper = stopMapper;
        _logger = logger;
    }

    public async Task Handle(LoadStaticGtfsCommand command, CancellationToken cancellation)
    {
        try
        {
            _transitDataReader.LoadStacks();

            var stops = new List<Stop>();
            var trips = new List<Trip>();

            _logger.LogInformation("Static gtfs data loaded in memory, mapping to aggregates...");

            while (_transitDataReader.Stops.TryPop(out var stop)) stops.Add(_stopMapper.MapFrom(stop));

            _logger.LogInformation("Stops mapped");

            while (_transitDataReader.Trips.TryPop(out var trip))
            {
                trips.Add(_tripMapper.MapFrom(trip));

                trip.Dispose();
            }

            _logger.LogInformation("Trips mapped");

            _transitDataReader.Dispose();

            _logger.LogInformation("Saving Aggregates, this is a long operation... (Between 1-5 minutes on most systems)");

            await _stopWriteRepository.AddAllAsync(stops.ToList());

            await _tripWriteRepository.AddAllAsync(trips.ToList());

            //using bulk inserts so save changes is not needed but it still dispatches the domain events
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while loading static gtfs data");
            throw;
        }
    }
}