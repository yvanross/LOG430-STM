using System.Net;
using Application.BusinessObjects;
using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Policies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using ServiceMeshHelper;
using ServiceMeshHelper.Bo;
using ServiceMeshHelper.Bo.InterServiceRequests;
using ServiceMeshHelper.Controllers;

namespace Infrastructure.Clients;

public class StmClient : IBusInfoProvider
{
    readonly ILogger _logger;
    private readonly IBackOffRetryPolicy<StmClient> _backOffRetry;
    private readonly IInfiniteRetryPolicy<StmClient> _infiniteRetry;

    public StmClient(ILogger<StmClient> logger, IBackOffRetryPolicy<StmClient> backOffRetry, IInfiniteRetryPolicy<StmClient> infiniteRetry)
    {
        _logger = logger;
        _backOffRetry = backOffRetry;
        _infiniteRetry = infiniteRetry;
    }

    public Task<IEnumerable<Ride>> GetBestBus(string startingCoordinates, string destinationCoordinates)
    {
        return _infiniteRetry.ExecuteAsync(async () =>
        {
            var channel = await RestController.Get(new GetRoutingRequest()
            {
                TargetService = "STM",
                Endpoint = $"Finder/OptimalBuses",
                Params = new List<NameValue>()
                {
                    new()
                    {
                        Name = "fromLatitudeLongitude",
                        Value = startingCoordinates
                    },
                    new()
                    {
                        Name = "toLatitudeLongitude",
                        Value = destinationCoordinates
                    },
                },
                Mode = LoadBalancingMode.RoundRobin
            });

            IEnumerable<Ride> busDto = Enumerable.Empty<Ride>();

            await foreach (var res in channel.ReadAllAsync())
            {
                busDto = JsonConvert.DeserializeObject<IEnumerable<Ride>>(res.Content);

                break;
            }

            return busDto;
        });
    }

    public Task BeginTracking(Ride stmBus)
    {
        return _infiniteRetry.ExecuteAsync(async () =>
        {
            _ = await RestController.Post(new PostRoutingRequest<Ride>()
            {
                TargetService = "STM",
                Endpoint = $"Track/BeginTracking",
                Payload = stmBus,
                Mode = LoadBalancingMode.RoundRobin
            });
        });
    }

    public Task<IBusTracking?> GetTrackingUpdate(string busId)
    {
        return _backOffRetry.ExecuteAsync<IBusTracking?>(async () =>
        {
            var channel = await RestController.Get(new GetRoutingRequest()
            {
                TargetService = "STM",
                Endpoint = $"Track/GetTrackingUpdate",
                Params = new List<NameValue>(),
                Mode = LoadBalancingMode.RoundRobin
            });

            RestResponse? data = null;

            await foreach (var res in channel.ReadAllAsync())
            {
                data = res;

                break;
            }

            if (data is null || !data.IsSuccessStatusCode || data.StatusCode.Equals(HttpStatusCode.NoContent)) return null;

            var busTracking = JsonConvert.DeserializeObject<BusTracking>(data.Content);

            return busTracking;

        });
    }
}