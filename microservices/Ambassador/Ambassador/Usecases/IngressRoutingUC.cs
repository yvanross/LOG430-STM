using Ambassador.BusinessObjects;
using Ambassador.Dto;
using Ambassador.Health;
using Ambassador.Properties;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace Ambassador.Usecases;

internal class IngressRoutingUC
{
    private protected static RestClient IngressClient { get; } = new(ContainerService.IngressAddress);

    private static HeartBeatService _heartBeatService = new(SendHeartbeat);

    internal async Task<RestResponse> Register(string serviceType, bool autoScaleInstances, int minimumNumberOfInstances)
    {
        var ingressAddress = ContainerService.IngressAddress;

        var serviceAddress = ContainerService.ServiceAddress;

        var client = new RestClient(ingressAddress);

        var request = new RestRequest(Resources.SubscribeToIngress_Endpoint, Method.Put);

        SubscriptionDto subscriptionDto = new (
            serviceType,
            serviceAddress,
            await ContainerService.GetContainerId(),
            ContainerService.ServiceId, autoScaleInstances,
            minimumNumberOfInstances);

        request.AddBody(JsonConvert.SerializeObject(subscriptionDto));

        request.AddHeader(KnownHeaders.Authorization, Resources.Authorization);

        var response = await client.ExecuteAsync(request);

        return response;
    }

    internal async Task<RoutingData?> GetServiceRoutingData(string targetService)
    {
        try
        {
            var request = new RestRequest(Resources.RouteByServiceType_Endpoint);

            request.AddQueryParameter("serviceType", targetService);

            request.AddHeader(KnownHeaders.Authorization, Resources.Authorization);

            var response = await IngressClient.ExecuteGetAsync<RoutingData>(request);

            response.ThrowIfError();

            return response.Data!;
        }
        catch (Exception)
        {
            ContainerService.Logger?.LogError("Data received from Ingress was problematic");

            throw;
        }
    }

    internal async Task BeginSendingHeartbeat()
    {
        try
        {
            _ = _heartBeatService.BeginSendingHeartbeats();
        }
        catch (Exception)
        {
            ContainerService.Logger?.LogError("Sending Heartbeat failed");

            throw;
        }
    }

    private static async Task SendHeartbeat()
    {
        var request = new RestRequest(Resources.HeartBeat_Endpoint, Method.Post);

        request.AddQueryParameter("serviceId", ContainerService.ServiceId);

        var response = await IngressClient.ExecutePostAsync(request);

        response.ThrowIfError();
    }
}