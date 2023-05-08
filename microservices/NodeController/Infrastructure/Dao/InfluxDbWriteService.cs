using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.OutGoing;
using Entities.DomainInterfaces.ResourceManagement;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using Newtonsoft.Json;
using Polly;

namespace Infrastructure.Dao;

public class InfluxDbWriteService : ISystemStateWriteService
{
    private readonly IHostInfo _hostInfo;
    private readonly ApplicationLogic.Usecases.Ingress _ingress;
    private readonly IPodReadService _podReadService;
    private static string _address = null!;

    private static InfluxDBClient? _client;

    private const string Org = "ets";

    public InfluxDbWriteService(IHostInfo hostInfo, ApplicationLogic.Usecases.Ingress ingress, IPodReadService podReadService)
    {
        _hostInfo = hostInfo;
        _ingress = ingress;
        _podReadService = podReadService;
    }

    public async Task Log(IExperimentReport experimentReport)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, delay, retryCount, _) =>
                {
                    Console.WriteLine($"Operation failed with exception: {exception.Message}. Waiting {delay} before next retry. Retry attempt {retryCount}.");
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            if (_client is null)
            {
                await _ingress.GetLogStoreAddressAndPort().ContinueWith(r => _address = r.Result);

                _client = new InfluxDBClient(new InfluxDBClientOptions($"http://{_address}")
                {
                    Bucket = _hostInfo.GetGroup(),
                    AllowHttpRedirects = true,
                    LogLevel = LogLevel.Basic,
                    Org = Org,
                    Username = "guest",
                    Password = "guest-pass"
                });
            }

            var dto = ExperimentReportDto.TryConvertToDto(experimentReport, _podReadService);

            var json = JsonConvert.SerializeObject(dto);

            var point = PointData
                .Measurement("experiment-report")
                .Field("latency", dto.AverageLatency)
                .Field("errors", dto.ErrorCount)
                .Field("report", json)
                .Tag("User", _hostInfo.GetUsername())
                .Timestamp(dto.Timestamp, WritePrecision.Ns);

            using var writeApi = _client.GetWriteApi();

            writeApi.WritePoint(point, _hostInfo.GetGroup(), Org);
        });
    }
}