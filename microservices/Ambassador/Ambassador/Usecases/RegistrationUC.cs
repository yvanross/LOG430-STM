using Ambassador.BusinessObjects;
using RestSharp;
using System.Net;

namespace Ambassador.Usecases;

public class RegistrationUC
{
    /// <summary>
    /// Registers this service in the Ingress
    /// </summary>
    /// <param name="serviceType"> For load balancing make sure your image in docker compose has the same name as this parameter </param>
    /// <returns></returns>
    public async Task Register(string serviceType)
    {
        var client = new RestClient(Properties.Resources.IngressAddressWithPort);

        var request = new RestRequest(Properties.Resources.SubscribeToIngress_Endpoint, Method.Put);

        request.AddQueryParameter("serviceType", serviceType);

        var response = await client.ExecuteAsync(request);

        response.ThrowIfError();
    }
}