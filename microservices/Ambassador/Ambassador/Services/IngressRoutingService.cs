using Ambassador.BusinessObjects;
using Ambassador.Health;
using Ambassador.Properties;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace Ambassador.Services;

internal class IngressRoutingService
{
    private protected static RestClient IngressClient { get; } = new(ContainerService.NodeControllerAddress);

    internal async Task<IEnumerable<RoutingData>> GetServiceRoutingData(string targetService, LoadBalancingMode routingRequestMode)
    {
        try
        {
            var request = new RestRequest("Routing/RouteByServiceType");

            request.AddQueryParameter("caller", ContainerService.ServiceId);

            request.AddQueryParameter("serviceType", targetService);

            request.AddQueryParameter("mode", routingRequestMode);

            var response = await IngressClient.ExecuteGetAsync<IEnumerable<RoutingData>>(request);

            response.ThrowIfError();

            return response.Data!;
        }
        catch (Exception)
        {
            ContainerService.Logger?.LogError("Data received from NodeController was problematic");

            throw;
        }
    }
}