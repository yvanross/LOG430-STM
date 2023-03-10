using Ambassador.BusinessObjects;
using RestSharp;
using System.Net;
using Ambassador.Extensions;
using Ambassador.Health;
using Ambassador.Usecases;
using Microsoft.Extensions.Logging;

namespace Ambassador.Controllers;

public static class RegistrationController
{
    private static IngressRoutingUC _ingressRoutingUc = new();

    /// <summary>
    /// Registers this service in the Ingress
    /// </summary>
    /// <param name="serviceType"> For load balancing make sure your image in docker compose has the same name as this parameter </param>
    /// <returns></returns>
    public static async Task Register(string serviceType, ILogger? logger, bool autoScaleInstances = true, int minimumNumberOfInstances = 1)
    {
        if (minimumNumberOfInstances > 10 || minimumNumberOfInstances < 1)
            minimumNumberOfInstances = 1;

        ContainerService.Logger = logger;

        await Try.WithConsequenceAsync(async () =>
        {
            logger?.LogInformation($"Attempting to subscribe service as {serviceType} to Ingress");

            var response = await _ingressRoutingUc.Register(serviceType, autoScaleInstances, minimumNumberOfInstances);

            response.ThrowIfError();

            logger?.LogInformation($"Subscription {response.StatusDescription} ");

            await _ingressRoutingUc.BeginSendingHeartbeat();

            return Task.CompletedTask;
        }, 
            retryCount: ContainerService.RetryCount,
            onFailure: (e, i) =>
            {
                logger?.LogError(
                $"{e.Message}\n {e.StackTrace} \n" +
                $"Ingress address: {ContainerService.IngressAddress} \n" +
                $"Service address: {ContainerService.ServiceAddress}");
                
                return Task.CompletedTask;
            });
    }
}