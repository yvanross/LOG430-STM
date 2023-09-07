using Application.Commands.Seedwork;
using Application.CommandServices;
using Application.CommandServices.Interfaces;
using Application.CommandServices.Repositories;
using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;

namespace Application.Commands.LoadStaticGtfs;

public sealed class LoadStaticGtfsHandler : ICommandHandler<LoadStaticGtfsCommand>
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
            _transitDataReader.LoadStaticGtfsFromFilesInMemory();

            _logger.LogInformation("Static gtfs data loaded in memory, mapping to aggregates...");

            var stops = new List<Stop>();

            foreach (var stopWrapper in _transitDataReader.FetchStopData())
            {
                stops.Add(_stopMapper.MapFrom(stopWrapper));
            }

            await _stopWriteRepository.AddAllAsync(stops);

            stops.Clear();

            GC.Collect();

            _logger.LogInformation("Stops persisted");

            await BatchInsertTrips(_transitDataReader.FetchTripData());

            GC.Collect();

            _logger.LogInformation("Trips persisted");

            _transitDataReader.Dispose();

            //for simplicity sake we don't use a transaction here but the events still only fire after the commit
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, 
                """
                        Error while loading static gtfs data.
                        This operation doesn't use transaction leaving the database in an inconsistent state.
                        Make sure to delete all data from the database before trying again.
                        """);

            throw;
        }
    }

    /// <summary>
    /// In order to spare computers with 8gb of ram (obviously only works with an actual database)
    /// Reducing Batch size lowers memory usage but increases time to insert
    /// Increasing Batch size increases memory usage but decreases time to insert
    /// You can always fine tune this to your liking, the current value takes into account systems with 8gb of ram
    /// </summary>
    /// <param name="aggregates"></param>
    /// <returns></returns>
    private async Task BatchInsertTrips(IEnumerable<ITripWrapper> tripWrappers)
    {
        const int batchSize = 10000;

        var batch = new List<Trip>(batchSize);

        foreach (var trip in tripWrappers)
        {
            batch.Add(_tripMapper.MapFrom(trip));

            trip.Dispose();

            if (batch.Count >= batchSize)
            {
                await _tripWriteRepository.AddAllAsync(batch);

                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await _tripWriteRepository.AddAllAsync(batch);

            batch.Clear();
        }
    }
}