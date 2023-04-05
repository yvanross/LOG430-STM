using ApplicationLogic.Interfaces.Dao;
using Entities.DomainInterfaces.ResourceManagement;
using ApplicationLogic.Usecases;
using NodeController.External.Docker;
using NodeController.External.Ingress;
using NodeController.Dto.OutGoing;
using Docker.DotNet.Models;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Newtonsoft.Json;
using Entities.BusinessObjects.ResourceManagement;
using System.Net;
using InfluxDB.Client.Writes;
using NodeController.Controllers;
using Polly;
using LogLevel = InfluxDB.Client.Core.LogLevel;

namespace NodeController.External.Dao;

public class InfluxDbWriteService : ISystemStateWriteService
{
    private static string _address = null!;

    private static readonly string Bucket = HostInfo.TeamName;

    private static InfluxDBClient? _client;

    private const string Org = "ets";

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
                await new IngressUC(new HostInfo(), new IngressClient())
                    .GetLogStoreAddressAndPort()
                    .ContinueWith(r => _address = r.Result);

                _client = new InfluxDBClient(new InfluxDBClientOptions($"http://{_address}")
                {
                    Bucket = Bucket,
                    AllowHttpRedirects = true,
                    LogLevel = LogLevel.Basic,
                    Org = Org,
                    Username = "guest",
                    Password = "guest-pass"
                });
            }

            var dto = ExperimentReportDto.TryConvertToDto(experimentReport);

            var json = JsonConvert.SerializeObject(dto);

            var point = PointData
                .Measurement("experiment-report")
                .Field("latency", dto.AverageLatency)
                .Field("errors", dto.ErrorCount)
                .Field("report", json)
                .Timestamp(dto.Timestamp, WritePrecision.Ns);

            using var writeApi = _client.GetWriteApi();

            writeApi.WritePoint(point, Bucket, Org);
        });
    }
}