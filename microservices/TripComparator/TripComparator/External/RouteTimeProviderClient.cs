using Ambassador;
using Ambassador.BusinessObjects;
using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador.Controllers;
using Entities.DomainInterfaces;
using Newtonsoft.Json;

namespace TripComparator.External;

public class RouteTimeProviderClient : IRouteTimeProvider
{
    public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
    {
        var res = await RestController.Get(new GetRoutingRequest()
        {
            TargetService = "RouteTimeProvider",
            Endpoint = $"RouteTime/Get",
            Params = new List<NameValue>()
            {
                new ()
                {
                    Name = "startingCoordinates",
                    Value = startingCoordinates
                },
                new ()
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

        return (int)times.Average();
    }
}