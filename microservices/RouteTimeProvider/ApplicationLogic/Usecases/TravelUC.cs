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
using Entities.DomainInterfaces;
using Newtonsoft.Json;
using RestSharp;

namespace ApplicationLogic.Usecases
{
    public class TravelUC
    {
        public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates, IRouteTimeProvider routeTimeProvider)
        {
            return await routeTimeProvider.GetTravelTimeInSeconds(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
        }
    }
}
