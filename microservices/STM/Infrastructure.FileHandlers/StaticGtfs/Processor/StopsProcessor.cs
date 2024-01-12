using System.Globalization;
using CsvHelper;
using Infrastructure.FileHandlers.StaticGtfs.Enum;
using Infrastructure.FileHandlers.StaticGtfs.Wrappers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileHandlers.StaticGtfs.Processor;

public sealed class StopsProcessor : IDisposable
{
    private readonly CsvReader _stopStreamCsv;

    public StopsProcessor(IDataReader dataReader, ILogger<StopsProcessor> logger)
    {
        var stopStream = dataReader.GetBinary(System.Enum.GetName(DataCategoryEnum.STOPS)!.ToLower());

        _stopStreamCsv = new CsvReader(stopStream, CultureInfo.InvariantCulture);
    }

    public async IAsyncEnumerable<StopWrapper> Process()
    {
        await _stopStreamCsv.ReadAsync();

        while (await _stopStreamCsv.ReadAsync())
        {
            var id = _stopStreamCsv.GetField<string>(0);

            var latitude = _stopStreamCsv.GetField<double>(1);

            var longitude = _stopStreamCsv.GetField<double>(2);

            yield return new StopWrapper(id, latitude, longitude);
        }
    }

    public void Dispose()
    {
        _stopStreamCsv.Dispose();
    }
}