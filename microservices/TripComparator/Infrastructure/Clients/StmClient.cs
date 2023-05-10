using System.Net;
using System.Runtime.Serialization;
using ApplicationLogic.Interfaces.Policies;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using ServiceMeshHelper;
using ServiceMeshHelper.Bo;
using ServiceMeshHelper.Bo.InterServiceRequests;
using ServiceMeshHelper.Controllers;
using TripComparator.DTO;

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

    public Task<IEnumerable<IStmBus?>> GetBestBus(string startingCoordinates, string destinationCoordinates)
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

            IEnumerable<IStmBus?> busDto = Enumerable.Empty<IStmBus>();

            await foreach (var res in channel.ReadAllAsync())
            {
                busDto = JsonConvert.DeserializeObject<IEnumerable<StmBusDto>>(res.Content);

                break;
            }

            return busDto;
        });
    }

    public Task BeginTracking(IStmBus? stmBus)
    {
        if(stmBus is not StmBusDto busDto) throw new InvalidDataContractException("Make sure to not alter the type stored in the collection returned by GetBestBus");

        return _infiniteRetry.ExecuteAsync(async () =>
        {
            _ = await RestController.Post(new PostRoutingRequest<StmBusDto>()
            {
                TargetService = "STM",
                Endpoint = $"Track/BeginTracking",
                Payload = busDto,
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
                Params = new List<NameValue>()
                {
                    new()
                    {
                        Name = "busId",
                        Value = busId
                    }
                },
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