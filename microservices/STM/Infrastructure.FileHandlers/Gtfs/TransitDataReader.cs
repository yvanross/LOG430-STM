using Application.CommandServices;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Common.Interfaces;
using Infrastructure.FileHandlers.Gtfs.Wrappers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileHandlers.Gtfs;

public class TransitDataReader : ITransitDataReader
{
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly GtfsFileFileCache _gtfsFileFileCache;

    private readonly ILogger<TransitDataReader> _logger;
    private readonly WrapperMediator _wrapperMediator;

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
    }

    public Stack<IStopWrapper> Stops { get; } = new();
    public Stack<ITripWrapper> Trips { get; } = new();

    public void LoadStacks()
    {
        _gtfsFileFileCache.LoadFileCache();

        FetchStopData();
        FetchTripData();
    }

    public void Dispose()
    {
        if (_disposed is false)
        {
            Trips.Clear();

            Stops.Clear();

            _wrapperMediator.Dispose();

            _gtfsFileFileCache.Dispose();

            _disposed = true;

            GC.Collect();

            GC.SuppressFinalize(_wrapperMediator);
            GC.SuppressFinalize(_gtfsFileFileCache);
            GC.SuppressFinalize(this);
        }
    }


    private void FetchStopData()
    {
        var stopsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOPS);

        foreach (var info in stopsInfo)
            try
            {
                var stop = new StopWrapper(info);

                _wrapperMediator.AddStop(stop);

                Stops.Push(stop);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"An error occurred while creating a stop, this is non fatal and trivial in small quantities. Exception: {e.Message}");
            }
    }

    private void FetchTripData()
    {
        var tripsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.TRIPS).ToList();

        foreach (var info in tripsInfo)
            try
            {
                var tripWrapper = new TripWrapper(info, _gtfsFileFileCache, _wrapperMediator, _datetimeProvider);

                Trips.Push(tripWrapper);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"An error occurred while creating a trip, this is non fatal and trivial in small quantities. Exception: {e.Message}");
            }
    }
}