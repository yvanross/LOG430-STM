using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RestSharp;

namespace ApplicationLogic.Usecases
{
    public class TravelUC
    {
        public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
        {
            var restWrapper = new RestWrapper();

            dynamic res = await restWrapper.Get(new GetRoutingRequest()
            {
                TargetService = ServiceTypes.Tomtom.ToString(),
                Endpoint = $"routing/1/calculateRoute/{HttpUtility.UrlEncode($"{startingCoordinates}:{destinationCoordinates}")}"
            });

            return res?.routes[0].summary.travelTimeInSeconds ?? default;
        }
    }
}
