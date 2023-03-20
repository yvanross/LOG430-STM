using ApplicationLogic.Extensions;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases;

public class Ingress
{
    private readonly ILogger _logger;

    public Ingress(ILogger logger)
    {
        _logger = logger;
    }

    public async Task Register()
    {
        await Try.WithConsequenceAsync(async () =>
            {
                _logger?.LogInformation($"Attempting to subscribe to IngressController");

                var response = await _ingressRoutingUc.Register(serviceType, autoScaleInstances, minimumNumberOfInstances);

                response.ThrowIfError();

                _logger?.LogInformation($"Subscription {response.StatusDescription} ");

                await _ingressRoutingUc.BeginSendingHeartbeat();

                return Task.CompletedTask;
            },
            retryCount: 100,
            onFailure: (e, i) =>
            {
                _logger?.LogError(
                    $"{e.Message}\n {e.StackTrace} \n" +
                    $"IngressController address: {ContainerService.IngressAddress} \n" +
                    $"Service address: {ContainerService.ServiceAddress}");

                return Task.CompletedTask;
            });
    }
}