using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using NodeController.External.Docker;
using RestSharp;

namespace NodeController.External.Ingress;

public class IngressClient : IIngressClient
{
    private static readonly string LogStoreAddressAndPort;

    public async Task Subscribe(string teamName, string address, string port)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var client = new RestClient($"{HostInfo.IngressAddress}:{HostInfo.IngressPort}");

            var request = new RestRequest($"Subscription/{teamName}");

            request.AddBody(new
            {
                address,
                port
            });

            var res = await client.ExecutePostAsync(request);

            res.ThrowIfError();

            return Task.CompletedTask;
        }, retryCount: 5);
    }

    public string GetLogStoreAddressAndPort()
    {
        return LogStoreAddressAndPort;
    }
}