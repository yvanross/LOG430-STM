using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Application.Mapping.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Aggregates;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.CommandServices.HostedServices;

public class LoadStaticGtfsHostedService : BackgroundService
{
    private readonly ITripWriteRepository _tripWriteRepository;
    private readonly IStopWriteRepository _stopWriteRepository;
    private readonly ITransitDataReader _transitDataReader;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingTo<ITripWrapper, Trip> _tripMapper;
    private readonly IMappingTo<IStopWrapper, Stop> _stopMapper;
    private readonly ILogger<LoadStaticGtfsHostedService> _logger;

    public LoadStaticGtfsHostedService(
        ITripWriteRepository tripWriteRepository,
        IStopWriteRepository stopWriteRepository,
        ITransitDataReader transitDataReader,
        IUnitOfWork unitOfWork,
        IMappingTo<ITripWrapper, Trip> tripMapper,
        IMappingTo<IStopWrapper, Stop> stopMapper,
        ILogger<LoadStaticGtfsHostedService> logger)
    {
        _tripWriteRepository = tripWriteRepository;
        _stopWriteRepository = stopWriteRepository;
        _transitDataReader = transitDataReader;
        _unitOfWork = unitOfWork;
        _tripMapper = tripMapper;
        _stopMapper = stopMapper;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var stops = _transitDataReader.Stops.Select(_stopMapper.MapFrom).ToList();

            await _stopWriteRepository.AddAllAsync(stops);

            var trips = _transitDataReader.Trips.Value.Select(_tripMapper.MapFrom);

            await _tripWriteRepository.AddAllAsync(trips);

            await _unitOfWork.SaveChangesAsync();

            _transitDataReader.Dispose();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while loading static gtfs data");
            throw;
        }
    }
}