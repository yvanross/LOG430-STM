using Application.Commands.Seedwork;
using Application.CommandServices.Interfaces;
using Application.CommandServices.Repositories;
using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Application.Commands.LoadStaticGtfs;

public sealed class LoadStaticGtfsHandler : ICommandHandler<LoadStaticGtfsCommand>
{
    private readonly int _batchSize;
    private readonly int _maxDegreeOfParallelism;

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
        IMemoryConsumptionSettings memoryConsumptionSettings,
        ILogger<LoadStaticGtfsHandler> logger)
    {
        _tripWriteRepository = tripWriteRepository;
        _stopWriteRepository = stopWriteRepository;
        _transitDataReader = transitDataReader;
        _unitOfWork = unitOfWork;
        _tripMapper = tripMapper;
        _stopMapper = stopMapper;
        _logger = logger;

        _batchSize = memoryConsumptionSettings.GetBatchSize();
        _maxDegreeOfParallelism = memoryConsumptionSettings.GetMaxDegreeOfParallelism();
    }

    public async Task Handle(LoadStaticGtfsCommand command, CancellationToken cancellation)
    {
        try
        {
            _logger.LogInformation("Loading Static gtfs data, should take around 1 to 5 minutes depending on configuration and hardware");

            await ProcessStops(cancellation);

            await ProcessTrips();

            GC.Collect();

            _transitDataReader.Dispose();

            //For simplicity, we don't use a transaction here but the events still only fire after the commit
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

    private async Task ProcessStops(CancellationToken cancellation)
    {
        var stops = new List<Stop>();

        await foreach (var stopWrapper in _transitDataReader.FetchStopData().WithCancellation(cancellation))
        {
            stops.Add(_stopMapper.MapFrom(stopWrapper));
        }

        await _stopWriteRepository.AddAllAsync(stops);

        _logger.LogInformation("Stops persisted");

        stops.Clear();
    }

    private async Task ProcessTrips()
    {
        var tripWrappers = _transitDataReader.FetchTripData();

        await BatchInsertTrips(tripWrappers);

        _logger.LogInformation("Trips persisted");
    }

    /// <summary>
    /// In order to spare computers with less ram (obviously only works with an actual database)
    /// Reducing Batch size lowers memory usage but increases time to insert
    /// Increasing Batch size increases memory usage but decreases time to insert
    /// You can always fine tune this to your liking, the current value takes into account most systems
    /// </summary>
    private async Task BatchInsertTrips(IAsyncEnumerable<ITripWrapper> tripWrappers)
    {
        var actionBlock = new ActionBlock<Trip[]>(
            async batch =>
            {
                // Each batch is processed in its own task with its own DbContext instance
                await _tripWriteRepository.AddAllAsync(batch);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _maxDegreeOfParallelism
            });

        var batch = new List<Trip>(_batchSize);

        await foreach (var trip in tripWrappers)
        {
            batch.Add(_tripMapper.MapFrom(trip));
            trip.Dispose();

            if (batch.Count >= _batchSize)
            {
                actionBlock.Post(batch.ToArray());

                batch = new List<Trip>(_batchSize);
            }
        }

        // Post the last batch if it has any trips
        if (batch.Count > 0)
        {
            actionBlock.Post(batch.ToArray());
        }

        // Ensure all batches are processed
        actionBlock.Complete();

        await actionBlock.Completion;
    }
}