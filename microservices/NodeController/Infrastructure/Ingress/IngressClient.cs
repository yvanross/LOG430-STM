using ApplicationLogic.Interfaces;
using ApplicationLogic.OutGoing;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace Infrastructure.Ingress;

public class IngressClient : IIngressClient
{
    private readonly IHostInfo _hostInfo;
    private readonly ILogger<IngressClient> _logger;

    private string _logStoreAddressAndPort = string.Empty;

    private readonly RestClient _client;

    public IngressClient(IHostInfo hostInfo, ILogger<IngressClient> logger)
    {
        _hostInfo = hostInfo;
        _logger = logger;

        _client = new($"http://{_hostInfo.GetIngressAddress()}:{_hostInfo.GetIngressPort()}");
    }

    public async Task Subscribe(string group, string teamName, string username, string secret, string address, string port)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(retryAttempt =>
                {
                    var time = TimeSpan.FromSeconds(retryAttempt + retryAttempt);

                    return time.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : time;
                },
            (exception, delay, retryCount) =>
            {
                _logger.LogError(exception, $"Most of the time this is because the VPN is not ON");
            });

        await retryPolicy.ExecuteAsync(async () =>
        {
            var request = new RestRequest($"Subscription/{group}/{teamName}/{username}");

            request.AddJsonBody(new IngressSubscriptionDto()
            {
                Secret = secret,
                Version = _hostInfo.GetVersion(),
            });

            var res = await _client.ExecutePostAsync(request);

            res.ThrowIfError();

            _logger.LogInformation($"Subscribed to Ingress");

            return Task.CompletedTask;
        });
    }

    public async Task<string> GetLogStoreAddressAndPort(string teamName)
    {
        if (string.IsNullOrEmpty(_logStoreAddressAndPort))
        {
            var request = new RestRequest($"LogStore/{teamName}");

            var res = await _client.GetAsync<string>(request);

            _logStoreAddressAndPort = res ?? string.Empty;
        }

        return _logStoreAddressAndPort;
    }
}