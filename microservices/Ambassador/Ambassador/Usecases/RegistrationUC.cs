using Ambassador.BusinessObjects;
using RestSharp;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Ambassador.Usecases;

public class RegistrationUC
{
    /// <summary>
    /// Registers this service in the Ingress
    /// </summary>
    /// <param name="serviceType"> For load balancing make sure your image in docker compose has the same name as this parameter </param>
    /// <returns></returns>
    public async Task Register(string serviceType, ILogger? logger)
    {
        try
        {
            EnvironmentVariables.Logger = logger;

            logger?.LogInformation($"Attempting to subscribe service as {serviceType} to Ingress");

            string ingressAddress = EnvironmentVariables.IngressAddress;

            string serviceAddress = EnvironmentVariables.ServiceAddress;

            // Read the contents of the metadata endpoint file
            var metadataEndpoint = "/proc/self/cgroup";

            var metadata = await File.ReadAllTextAsync(metadataEndpoint);

            // Extract the container ID from the metadata
            var containerId = metadata.Split('\n')
                .FirstOrDefault(line => line.Contains("docker"))
                ?.Split('/')
                .LastOrDefault();

            if (containerId is null)
            {
                logger?.LogError("Unable to determine the container ID. Service not connected to Ingress");
                return;
            }

            var client = new RestClient(ingressAddress);

            var request = new RestRequest(Properties.Resources.SubscribeToIngress_Endpoint, Method.Put);

            request.AddQueryParameter("serviceType", serviceType);

            request.AddQueryParameter("serviceAddress", serviceAddress);

            request.AddQueryParameter("containerId", containerId);

            var response = await client.ExecuteAsync(request);
            
            response.ThrowIfError();

            logger?.LogInformation($"Subscription {response.StatusDescription} ");
        }
        catch (Exception e)
        {
            logger?.LogError($"{e.Message}\n {e.StackTrace} \n Ingress address: {EnvironmentVariables.IngressAddress} \n Service address: {EnvironmentVariables.ServiceAddress}");
            throw;
        }
        
    }
}