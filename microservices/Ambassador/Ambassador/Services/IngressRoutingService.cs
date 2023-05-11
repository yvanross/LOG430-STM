using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using ServiceMeshHelper.Bo;

namespace ServiceMeshHelper.Services;

internal class IngressRoutingService
{
    private protected static RestClient NodeControllerClient { get; } = new(ContainerService.NodeControllerAddress);

    internal async Task<IEnumerable<RoutingData>>  GetServiceRoutingData(string targetService, LoadBalancingMode routingRequestMode)
    {

        var request = new RestRequest($"Routing/{targetService}");

        request.AddQueryParameter("caller", ContainerService.ServiceId);

        request.AddQueryParameter("mode", routingRequestMode);

        var response = await NodeControllerClient.ExecuteGetAsync<IEnumerable<RoutingData>>(request);

        response.ThrowIfError();

        return response.Data!;
    }
}