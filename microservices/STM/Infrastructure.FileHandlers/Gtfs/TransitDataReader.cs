﻿using Application.CommandServices.ServiceInterfaces;
using Application.Mapping.Interfaces.Wrappers;
using Domain.Common.Interfaces;
using Infrastructure.FileHandlers.Gtfs.Wrappers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileHandlers.Gtfs;

public class TransitDataReader : ITransitDataReader
{
    public Stack<IStopWrapper> Stops { get; } = new();
    public Stack<ITripWrapper> Trips { get; } = new();

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

        FetchStopData();
        FetchTripData();
    }

    private void FetchStopData()
    {
        var stopsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOPS);

        foreach (var info in stopsInfo)
        {
            try
            {
                var stop = new StopWrapper(info);

                _wrapperMediator.AddStop(stop);

                Stops.Push(stop);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while creating stop. Exception: {e.Message}");
            }
          
        }
    }

    private void FetchTripData()
    {
        var tripsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.TRIPS).ToList();

        foreach (var info in tripsInfo)
        {
            try
            {
                var tripWrapper = new TripWrapper(info, _gtfsFileFileCache, _wrapperMediator, _datetimeProvider);

                Trips.Push(tripWrapper);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while creating trip. Exception: {e.Message}");
            }
        }
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
}