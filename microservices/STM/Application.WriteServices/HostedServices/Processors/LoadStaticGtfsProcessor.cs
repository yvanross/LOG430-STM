using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Application.EventHandlers.AntiCorruption;
using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Contracts;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices.Processors;

public class LoadStaticGtfsProcessor : IScopedProcessor
{
    private readonly ITripWriteRepository _tripWriteRepository;
    private readonly IStopWriteRepository _stopWriteRepository;
    private readonly ITransitDataReader _transitDataReader;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingTo<ITripWrapper, Trip> _tripMapper;
    private readonly IMappingTo<IStopWrapper, Stop> _stopMapper;
    private readonly ILogger<LoadStaticGtfsProcessor> _logger;
    private readonly IPublisher _publisher;

    public LoadStaticGtfsProcessor(
        ITripWriteRepository tripWriteRepository,
        IStopWriteRepository stopWriteRepository,
        ITransitDataReader transitDataReader,
        IUnitOfWork unitOfWork,
        IMappingTo<ITripWrapper, Trip> tripMapper,
        IMappingTo<IStopWrapper, Stop> stopMapper,
        ILogger<LoadStaticGtfsProcessor> logger,
        IPublisher publisher)
    {
        _tripWriteRepository = tripWriteRepository;
        _stopWriteRepository = stopWriteRepository;
        _transitDataReader = transitDataReader;
        _unitOfWork = unitOfWork;
        _tripMapper = tripMapper;
        _stopMapper = stopMapper;
        _logger = logger;
        _publisher = publisher;
    }

    public async Task ProcessUpdates()
    {
        try
        {
            var stops = new List<Stop>();
            var trips = new List<Trip>();

            while (_transitDataReader.Stops.TryPop(out var stop))
            {
                stops.Add(_stopMapper.MapFrom(stop));
            }

            while (_transitDataReader.Trips.TryPop(out var trip))
            {
                trips.Add(_tripMapper.MapFrom(trip));

                trip.Dispose();
            }

            _transitDataReader.Dispose();

            await _stopWriteRepository.AddAllAsync(stops.ToList());

            await _tripWriteRepository.AddAllAsync(trips.ToList());

            await _unitOfWork.SaveChangesAsync();

            _publisher.Publish(new StaticGtfsDataLoaded());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while loading static gtfs data");
            throw;
        }
    }
}