using RestSharp;
using ServiceMeshHelper.BusinessObjects;
using ServiceMeshHelper.Services;

namespace ServiceMeshHelper.Clients;

internal class NodeControllerRoutingClient
{
    private static RestClient NodeControllerClient { get; } = new(ServiceMeshConfiguration.NodeControllerAddress);

    internal async Task<IEnumerable<RoutingData>> RouteByServiceType(string serviceType, LoadBalancingMode routingRequestMode)
        => await Route(serviceType, routingRequestMode);

    internal async Task<int> NegotiateSocketForServiceType(string serviceType)
        => await NegotiateSocket(serviceType);

    private async Task<IEnumerable<RoutingData>> Route(string serviceType, LoadBalancingMode routingRequestMode)
    {
        var request = new RestRequest($"Routing/RouteByServiceType/{serviceType}");

        request.AddQueryParameter("caller", ServiceMeshConfiguration.ServiceId);

        request.AddQueryParameter("mode", routingRequestMode);

        var response = await NodeControllerClient.ExecuteAsync<IEnumerable<RoutingData>>(request);

        response.ThrowIfError();

        return response.Data!;
    }

    private async Task<int> NegotiateSocket(string serviceType)
    {
        var request = new RestRequest($"Routing/NegotiateSocket/{serviceType}", Method.Post);

        var response = await NodeControllerClient.ExecuteAsync<int>(request);

        response.ThrowIfError();

        return response.Data!;
    }
}