using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ambassador.Controllers;
using Newtonsoft.Json;
using RestSharp;

namespace ApplicationLogic.Usecases
{
    public class TravelUC
    {
        public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
        {

            var res = await RestController.Get(new GetRoutingRequest()
            {
                TargetService = ServiceTypes.Tomtom.ToString(),
                Endpoint = $"routing/1/calculateRoute/{HttpUtility.UrlEncode($"{startingCoordinates}:{destinationCoordinates}")}/json",
            });

            var result = await res.ReadAsync();

            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(result.Content);

            return (int)(data?.routes[0].summary.travelTimeInSeconds ?? default);
        }
    }
}
