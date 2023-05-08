using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using InfluxDB.Client;
using Microsoft.Extensions.Logging;
using Polly;
using LogLevel = InfluxDB.Client.Core.LogLevel;

namespace Infrastructure.Clients;

public class InfluxDbReadService : ISystemStateReadService
{
    private readonly IHostInfo _hostInfo;
    private readonly ILogger<InfluxDBClient> _logger;
    private static InfluxDBClient? _client;

    private static string _address = null!;
    private const string Org = "ets";

    public InfluxDbReadService(IHostInfo hostInfo, ILogger<InfluxDBClient> logger)
    {
        _hostInfo = hostInfo;
        _logger = logger;
    }

    public async Task<ConcurrentDictionary<string, object?>> GetStates(IEnumerable<string> names, string group)
    {
        await EstablishConnection(group);

        var queryable = _client.GetQueryApi();

        var reports = new ConcurrentDictionary<string, object?>();

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)),
                (exception, delay, retryCount, _) =>
                {
                    _logger.LogCritical($"Operation failed with exception: {exception.Message}. Waiting {delay} before next retry. Retry attempt {retryCount}.");
                });

        try
        {
            await Parallel.ForEachAsync(names, async (name, cancellationToken) =>
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    var res = await queryable.QueryAsync(GetQuery(group, name), Org, cancellationToken);

                    var bucketReport = res?
                        .SelectMany(table => table.Records)?
                        .Select(record => record.Values["_value"])
                        .FirstOrDefault();

                    reports.AddOrUpdate(name, _ => bucketReport, (_, _) => bucketReport);
                });
            });
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.ToString());
        }

        static string GetQuery(string group, string name) =>
        $"""
            from(bucket: "{group}")
            |> range(start: -1m)
            |> filter(fn: (r) => r._measurement == "experiment-report")
            |> filter(fn: (r) => r._field == "report")
            |> filter(fn: (r) => r.User == "{name}")
            |> last()
        """;

        return reports;
    }

    private async Task EstablishConnection(string group)
    {
        if (_client is not null) return;

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, delay, retryCount, _) =>
                {
                    Console.WriteLine(
                        $"Operation failed with exception: {exception.Message}. Waiting {delay} before next retry. Retry attempt {retryCount}.");
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            if (_client is null)
            {
                _address = $"{_hostInfo.GetAddress()}:{_hostInfo.GetNodeStateStoragePort()}";

                _client = new InfluxDBClient(new InfluxDBClientOptions($"http://{_address}")
                {
                    Bucket = group,
                    AllowHttpRedirects = true,
                    LogLevel = LogLevel.Basic,
                    Org = Org,
                    Token = _hostInfo.GetNodeStateStorageToken()
                });
            }
        });
    }
}