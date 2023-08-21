using Domain.Services.Aggregates;
using Domain.ValueObjects;
using System.Collections.Immutable;
using Application.QueryServices.ServiceInterfaces;
using Domain.Aggregates.Stop;
using Microsoft.Extensions.Logging;

namespace Application.QueryServices;

public class ApplicationStopService
{
    private readonly IQueryRepository _readStops;
    private readonly StopServices _stopServices;
    private readonly ILogger<ApplicationStopService> _logger;

    public ApplicationStopService(IQueryRepository readStops, StopServices stopServices, ILogger<ApplicationStopService> logger)
    {
        _readStops = readStops;
        _stopServices = stopServices;
        _logger = logger;
    }

    public async Task<ImmutableHashSet<Stop>> GetClosestStops(Position position)
    {
        try
        {
            const int radiusForBusStopSelectionInMeters = 50;

            var stops = _readStops.GetData<Stop>().ToList();

            var closestStop = _stopServices.FindClosestStop(position, stops);

            var closestStops = _stopServices.GetNearbyStops(closestStop.Position, stops, radiusForBusStopSelectionInMeters);

            return closestStops.ToImmutableHashSet();
        }
        catch (Exception e)
        { 
            _logger.LogError(e, "Error while getting closest stops");

            throw;
        }
        
    }
}