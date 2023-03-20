using Ambassador.BusinessObjects;
using Ambassador.Dto;
using Ambassador.Health;
using Ambassador.Properties;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace Ambassador.Usecases;

internal class IngressService
{
    private static HeartBeatService _heartBeatService = new();

    internal async Task<RestResponse> Register(string serviceType, bool autoScaleInstances, int minimumNumberOfInstances)
    {
        var ingressAddress = ContainerService.IngressAddress;

        var serviceAddress = ContainerService.ServiceAddress;

        var client = new RestClient(ingressAddress);

        var request = new RestRequest(ContainerService.FormatIngressRequest(ContainerService.SubscriptionController, Resources.SubscribeToIngress_Endpoint), Method.Put);

        ContainerService.Logger?.LogInformation($"Subscribing to {ingressAddress}/{ContainerService.FormatIngressRequest(ContainerService.SubscriptionController, Resources.SubscribeToIngress_Endpoint)}");

        SubscriptionDto subscriptionDto = new (
            serviceType,
            serviceAddress,
            await ContainerService.GetContainerId(),
            ContainerService.ServiceId, 
            autoScaleInstances,
            minimumNumberOfInstances);

        request.AddBody(JsonConvert.SerializeObject(subscriptionDto));

        var response = await client.ExecuteAsync(request);

        if(response.IsSuccessStatusCode) 
            _heartBeatService.TryBeginSendingHeartbeats(SendHeartbeat);

        return response;
    }

    internal async Task<IEnumerable<RoutingData>> GetServiceRoutingData(string targetService, LoadBalancingMode routingRequestMode)
    {
        try
        {
            var request = new RestRequest(ContainerService.FormatIngressRequest(ContainerService.IngressController, Resources.RouteByServiceType_Endpoint));

            request.AddQueryParameter("serviceType", targetService);
            
            request.AddQueryParameter("mode", routingRequestMode);

            var response = await IngressClient.ExecuteGetAsync<IEnumerable<RoutingData>>(request);

            response.ThrowIfError();

            return response.Data!;
        }
        catch (Exception)
        {
            ContainerService.Logger?.LogError("Data received from IngressController was problematic");

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
        try
        {
            var request = new RestRequest(ContainerService.FormatIngressRequest(ContainerService.IngressController, Resources.HeartBeat_Endpoint), Method.Post);

            request.AddQueryParameter("serviceId", ContainerService.ServiceId);

            var response = await IngressClient.ExecutePostAsync(request);

            response.ThrowIfError();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            ContainerService.Logger?.LogError("HeartBeat failed");

            throw;
        }
        
    }
}