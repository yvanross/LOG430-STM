using RestSharp;
using ServiceMeshHelper.Bo;

namespace ServiceMeshHelper.Services;

internal class NodeControllerRoutingClient
{
    private static RestClient NodeControllerClient { get; } = new(ContainerService.NodeControllerAddress);

    internal async Task<IEnumerable<RoutingData>> RouteByServiceType(string serviceType, LoadBalancingMode routingRequestMode)
    {
        var request = new RestRequest($"Routing/RouteByServiceType/{serviceType}");

        request.AddQueryParameter("caller", ContainerService.ServiceId);

        request.AddQueryParameter("mode", routingRequestMode);

        var response = await NodeControllerClient.ExecuteAsync<IEnumerable<RoutingData>>(request);

        response.ThrowIfError();

        return response.Data!;
    }

    internal async Task<int> NegotiateSocket(string serviceType)
    {
        var request = new RestRequest($"Routing/NegotiateSocket/{serviceType}", Method.Post);

        var response = await NodeControllerClient.ExecuteAsync<int>(request);

        response.ThrowIfError();

        return response.Data!;
    }
}