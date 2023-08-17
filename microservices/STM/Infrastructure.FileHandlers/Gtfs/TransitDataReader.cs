using Application.CommandServices.ServiceInterfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Common.Interfaces;
using Infrastructure.FileHandlers.Gtfs.Wrappers;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace Infrastructure.FileHandlers.Gtfs;

public class TransitDataReader : ITransitDataReader
{
    public ImmutableList<IStopWrapper> Stops { get; private set; }

    public Lazy<ImmutableList<ITripWrapper>> Trips { get; private set; }

    private readonly ILogger<TransitDataReader> _logger;
    private readonly GtfsFileFileCache _gtfsFileFileCache;
    private readonly WrapperMediator _wrapperMediator;
    private readonly IDatetimeProvider _datetimeProvider;

    private bool _disposed;

    public TransitDataReader(
        ILogger<TransitDataReader> logger,
        GtfsFileFileCache gtfsFileFileCache,
        WrapperMediator wrapperMediator,
        IDatetimeProvider datetimeProvider)
    {
        _logger = logger;
        _gtfsFileFileCache = gtfsFileFileCache;
        _wrapperMediator = wrapperMediator;
        _datetimeProvider = datetimeProvider;

        Stops = FetchStopData();
        Trips = new(FetchTripData);
    }

    private ImmutableList<IStopWrapper> FetchStopData()
    {
        var stopsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOPS);

        foreach (var info in stopsInfo)
        {
            var stop = new StopWrapper(info);

            _wrapperMediator.AddStop(stop);
        }

        return _wrapperMediator.Stops.Values.ToImmutableList();
    }

    private ImmutableList<ITripWrapper> FetchTripData()
    {
        var tripsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.TRIPS).ToList();

        return tripsInfo.Select(info => (ITripWrapper)new TripWrapper(info, _gtfsFileFileCache, _wrapperMediator, _datetimeProvider)).ToImmutableList();
    }

    public void Dispose()
    {
        if (_disposed is false)
        {
            if (Trips.IsValueCreated)
                Trips = new(() => ImmutableList<ITripWrapper>.Empty);

            Stops = ImmutableList<IStopWrapper>.Empty;

            _wrapperMediator.Dispose();

            _gtfsFileFileCache.Dispose();

            _disposed = true;
        }
    }
}