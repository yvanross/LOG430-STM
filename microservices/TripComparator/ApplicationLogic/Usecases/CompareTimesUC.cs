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

namespace ApplicationLogic.Usecases
{
    public class CompareTimesUC
    {
        public async Task<int> CompareBusAndCarTime(string startingCoordinates, string destinationCoordinates)
        {
            var restWrapper = new RestWrapper();

            var res = await restWrapper.Get(new GetRoutingRequest()
            {
                TargetService = ServiceTypes.RouteTimeProvider.ToString(),
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
                }
            });

            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(res.Content);

            return (int)(data?.routes[0].summary.travelTimeInSeconds ?? default);
        }
    }
}
