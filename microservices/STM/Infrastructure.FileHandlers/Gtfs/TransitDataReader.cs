using Application.CommandServices.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Common.Interfaces;
using Infrastructure.FileHandlers.Gtfs.Wrappers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileHandlers.Gtfs;

public class TransitDataReader : ITransitDataReader
{
    private readonly GtfsFileFileCache _gtfsFileFileCache;

    private readonly ILogger<TransitDataReader> _logger;
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
    }

    public void LoadStaticGtfsFromFilesInMemory()
    {
        _gtfsFileFileCache.LoadFileCache();
    }

    public void Dispose()
    {
        if (_disposed is false)
        {
            _wrapperMediator.Dispose();

            _gtfsFileFileCache.Dispose();

            _disposed = true;

            GC.Collect();

            GC.SuppressFinalize(_wrapperMediator);
            GC.SuppressFinalize(_gtfsFileFileCache);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// This needs to be called before FetchTripData
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IStopWrapper> FetchStopData()
    {
        var stopsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOPS);

        foreach (var info in stopsInfo)
        {
            IStopWrapper stopWrapper;

            try
            {
                var stop = new StopWrapper(info);

                _wrapperMediator.AddStop(stop);

                stopWrapper = stop;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"An error occurred while creating a stop, this is non fatal and trivial in small quantities. Exception: {e.Message}");

                continue;
            }

            yield return stopWrapper;
        }
           


    }

    public IEnumerable<ITripWrapper> FetchTripData()
    {
        var tripsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.TRIPS).ToList();

        foreach (var info in tripsInfo)
        {
            ITripWrapper tripWrapper;
            try
            {
                tripWrapper = new TripWrapper(info, _gtfsFileFileCache, _wrapperMediator, _datetimeProvider);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"An error occurred while creating a trip, this is non fatal and trivial in small quantities. Exception: {e.Message}");

                continue;
            }

            yield return tripWrapper;
        }
            
    }
}