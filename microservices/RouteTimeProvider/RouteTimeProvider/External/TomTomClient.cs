using System.Dynamic;
using Entities.DomainInterfaces;
using RestSharp;
using System.Web;
using Newtonsoft.Json;

namespace PLACEHOLDER.External;

public class TomTomClient : IRouteTimeProvider
{
    private RestClient _restClient = new ("https://api.tomtom.com");

    private string _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? string.Empty;

    public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
    {
        var request = new RestRequest($"routing/1/calculateRoute/{HttpUtility.UrlEncode($"{startingCoordinates}:{destinationCoordinates}")}/json");

        request.AddHeader("key", _apiKey);

        var result = await _restClient.ExecuteGetAsync(request);

        dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(result.Content);

        return (int)(data?.routes.FirstOrDefault()?.summary.travelTimeInSeconds ?? default)!;
    }
}