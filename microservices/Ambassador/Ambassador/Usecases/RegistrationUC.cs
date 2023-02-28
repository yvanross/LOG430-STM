using Ambassador.BusinessObjects;
using RestSharp;
using System.Net;

namespace Ambassador.Usecases;

public class RegistrationUC
{
    public async Task Register(string serviceType)
    {
        var client = new RestClient(Properties.Resources.IngressAddressWithPort);

        var request = new RestRequest(Properties.Resources.SubscribeToIngress_Endpoint, Method.Put);

        request.AddQueryParameter("serviceType", serviceType);

        var response = await client.ExecuteAsync(request);

        response.ThrowIfError();
    }
}