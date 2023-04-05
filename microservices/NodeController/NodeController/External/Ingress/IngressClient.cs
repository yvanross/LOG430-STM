using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using NodeController.Dto.OutGoing;
using NodeController.External.Docker;
using RestSharp;

namespace NodeController.External.Ingress;

public class IngressClient : IIngressClient
{
    private string _logStoreAddressAndPort = string.Empty;

    private static readonly RestClient Client = new ($"http://{HostInfo.IngressAddress}:{HostInfo.IngressPort}");

    public async Task Subscribe(string teamName, string address, string port)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"Subscription/{teamName}");

            request.AddJsonBody(new IngressSubsciptionDto()
            {
                Address = address,
                Port = port
            });

            var res = await Client.ExecutePostAsync(request);

            res.ThrowIfError();

            return Task.CompletedTask;
        }, retryCount: 5);
    }

    public async Task<string> GetLogStoreAddressAndPort(string teamName)
    {
        if (string.IsNullOrEmpty(_logStoreAddressAndPort))
        {
            var request = new RestRequest($"LogStore/{teamName}");

            var res = await Client.GetAsync<string>(request);

            _logStoreAddressAndPort = res ?? string.Empty;
        }

        return _logStoreAddressAndPort;
    }
}