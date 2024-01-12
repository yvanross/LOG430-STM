using System.Globalization;
using Application.CommandServices.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using CsvHelper;
using Infrastructure.FileHandlers.StaticGtfs.Enum;
using Infrastructure.FileHandlers.StaticGtfs.Mappers.TypeConverter;
using Infrastructure.FileHandlers.StaticGtfs.Wrappers;
using Microsoft.Extensions.Logging;
using TripWrapper = Infrastructure.FileHandlers.StaticGtfs.Wrappers.TripWrapper;

namespace Infrastructure.FileHandlers.StaticGtfs.Processor;

public sealed class TripProcessor : IDisposable
{
    private readonly int _batchSize;

    private readonly GtfsTimespanConverter _gtfsTimespanConverter;

    private readonly IDataReader _dataReader;

    private CsvReader? _stopTimesStreamCsv;

    private readonly Queue<string> _tripsToProcess = new();

    public TripProcessor(IDataReader dataReader, ILogger<TripProcessor> logger, GtfsTimespanConverter gtfsTimespanConverter, IMemoryConsumptionSettings consumptionSettings)
    {
        try
        {
            _batchSize = consumptionSettings.FileReadBatchSize();

            _gtfsTimespanConverter = gtfsTimespanConverter;

            _dataReader = dataReader;

            var tripStream = _dataReader.GetBinary(System.Enum.GetName(DataCategoryEnum.TRIPS)!.ToLower());

            var tripCsv = new CsvReader(tripStream, CultureInfo.InvariantCulture);

            tripCsv.Read();

            while (tripCsv.Read())
            {
                var id = tripCsv.GetField<string>(0);

                if (string.IsNullOrWhiteSpace(id))
                {
                    logger.LogWarning($"Trip id is null or empty, continuing...");

                    continue;
                }

                _tripsToProcess.Enqueue(id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to process trips");

            throw;
        }
    }

    private void CreateStopTimeStream()
    {
        var stopTimesStream = _dataReader.GetBinary(System.Enum.GetName(DataCategoryEnum.STOP_TIMES)!.ToLower());

        _stopTimesStreamCsv = new CsvReader(stopTimesStream, CultureInfo.InvariantCulture);
    }

    public async IAsyncEnumerable<TripWrapper> Process()
    {
        while (_tripsToProcess.Count > 0)
        {
            CreateStopTimeStream();

            var trips = new Dictionary<string, TripWrapper>();

            while (_tripsToProcess.Count > 0 && trips.Count < _batchSize)
            {
                var tripId = _tripsToProcess.Dequeue();

                trips.Add(tripId, new TripWrapper(tripId, new List<IStopScheduleWrapper>()));
            }

            await _stopTimesStreamCsv!.ReadAsync().ConfigureAwait(false);
            
            while (await _stopTimesStreamCsv.ReadAsync().ConfigureAwait(false))
            {
                var id = _stopTimesStreamCsv.GetField<string>(0);

                if (trips.TryGetValue(id, out var tripWrapper))
                {
                    var stopId = _stopTimesStreamCsv.GetField<string>(2);
                    var departure = _stopTimesStreamCsv.GetField<string>(1);
                    var stopSequence = _stopTimesStreamCsv.GetField<int>(3);

                    var departureTime = _gtfsTimespanConverter.ConvertFromString(departure);

                    var stopScheduleWrapper = new StopScheduleWrapper(stopId, departureTime, stopSequence);

                    if(stopScheduleWrapper is null) throw new NullReferenceException("StopScheduleWrapper was null");

                    tripWrapper.ScheduledStops.Add(stopScheduleWrapper);
                }
            }

            foreach (var trip in trips)
            {
                trip.Value.ScheduledStops.Sort((x, y) => x.StopSequence.CompareTo(y.StopSequence));

                yield return trip.Value;
            }

            trips.Clear();
        }
    }

    public void Dispose()
    {
        _stopTimesStreamCsv?.Dispose();

        _dataReader.Dispose();

        _tripsToProcess.Clear();

        GC.SuppressFinalize(this);
    }

    ~TripProcessor()
    {
        Dispose();
    }
}