using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ambassador.BusinessObjects;
using Ambassador.Controllers;

namespace ApplicationLogic.Usecases
{
    public class CompareTimesUC
    {
        public async Task<int> CompareBusAndCarTime(string startingCoordinates, string destinationCoordinates)
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
}
