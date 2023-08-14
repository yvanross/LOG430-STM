using ApplicationLogic.Interfaces.Policies;
using Entities.DomainInterfaces;
using Newtonsoft.Json;
using RestSharp;
using ServiceMeshHelper;
using ServiceMeshHelper.Bo;
using ServiceMeshHelper.Bo.InterServiceRequests;
using ServiceMeshHelper.Controllers;

namespace Infrastructure.Clients;

public class RouteTimeProviderClient : IRouteTimeProvider
{
    private readonly IInfiniteRetryPolicy<RouteTimeProviderClient> _infiniteRetry;

    public RouteTimeProviderClient(IInfiniteRetryPolicy<RouteTimeProviderClient> infiniteRetry)
    {
        _infiniteRetry = infiniteRetry;
    }
    
    public Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
    {
        return _infiniteRetry.ExecuteAsync(async () =>
        {
            /*var res = await RestController.Get(new GetRoutingRequest()
            {
                TargetService = "RouteTimeProvider",
                Endpoint = $"RouteTime/Get",
                Params = new List<NameValue>()
                {
                    new()
                    {
                        Name = "startingCoordinates",
                        Value = startingCoordinates
                    },
                    new()
                    {
                        Name = "destinationCoordinates",
                        Value = destinationCoordinates
                    },
                },
                Mode = LoadBalancingMode.Broadcast
            });

            var times = new List<int>();

            await foreach (var result in res!.ReadAllAsync())
            {
                times.Add(JsonConvert.DeserializeObject<int>(result.Content));
            }

            return (int)times.Average();*/

            var restClient = new RestClient("http://RouteTimeProvider");
            var restRequest = new RestRequest("RouteTime/Get");

            restRequest.AddQueryParameter("startingCoordinates", startingCoordinates);
            restRequest.AddQueryParameter("destinationCoordinates", destinationCoordinates);
            
            return (await restClient.ExecuteGetAsync<int>(restRequest)).Data;
        });
    }
}