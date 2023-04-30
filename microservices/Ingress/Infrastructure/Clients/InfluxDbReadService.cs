using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using InfluxDB.Client;
using Polly;
using LogLevel = InfluxDB.Client.Core.LogLevel;

namespace Infrastructure.Clients;

public class InfluxDbReadService : ISystemStateReadService
{
    private readonly IHostInfo _hostInfo;
    private static string _address = null!;

    private static InfluxDBClient? _client;

    private const string Org = "ets";

    public InfluxDbReadService(IHostInfo hostInfo)
    {
        _hostInfo = hostInfo;
    }

    public async Task<IEnumerable<object>> ReadLogs(IEnumerable<string> names)
    {
        await EstablishConnection();

        var queryable = _client.GetQueryApi();

        var reports = ImmutableList<object>.Empty;

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)),
                (exception, delay, retryCount, _) =>
                {
                    Console.WriteLine($"Operation failed with exception: {exception.Message}. Waiting {delay} before next retry. Retry attempt {retryCount}.");
                });


        await Parallel.ForEachAsync(names, async (name,cancellationToken) =>
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                var res = await queryable.QueryAsync(GetQuery(name), Org, cancellationToken);

                var bucketReports = res
                    .SelectMany(table => table.Records)
                    .Select(record => record.GetValueByKey("report"));

                ImmutableInterlocked.Update(ref reports, (old) => old.AddRange(bucketReports));
            });
        });

        static string GetQuery(string name) =>
        $"""
            from(bucket: {name}) |> range(start: -1h) |> filter(fn: (r) => r._measurement == experiment-report) |> filter(fn: (r) => r._field == report)
        """;

        return reports.ToList();
    }

    private async Task EstablishConnection()
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
                    AllowHttpRedirects = true,
                    LogLevel = LogLevel.Basic,
                    Org = Org,
                    Username = "guest",
                    Password = "guest-pass"
                });
            }
        });
    }
}