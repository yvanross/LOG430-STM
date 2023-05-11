using ApplicationLogic.Interfaces;
using ApplicationLogic.OutGoing;
using Polly;
using RestSharp;

namespace Infrastructure.Ingress;

public class IngressClient : IIngressClient
{
    private readonly IHostInfo _hostInfo;

    private string _logStoreAddressAndPort = string.Empty;

    private readonly RestClient _client;

    public IngressClient(IHostInfo hostInfo)
    {
        _hostInfo = hostInfo;

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
                Console.WriteLine($"Operation failed with exception: {exception.Message}. Waiting {delay} before next retry. Retry attempt {retryCount}.");
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

            Console.WriteLine($"Subscribed to Ingress");

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