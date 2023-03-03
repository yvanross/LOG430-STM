using ApplicationLogic.Interfaces;
using RestSharp;

namespace STM.External;

public class CallBackClient : ICallBackClient
{
    public string Key { get; }

    private readonly RestClient _client;

    public CallBackClient(string callBackAddress)
    {
        _client = new RestClient(callBackAddress);
    }

    public async Task CallBack(string message, string deltaTime, bool closingConnection)
    {
        var request = new RestRequest();

        request.AddQueryParameter("key", Key);
        request.AddQueryParameter("message", message);
        request.AddQueryParameter("deltaTime", deltaTime);
        request.AddQueryParameter("closingConnection", closingConnection);

        _ = await _client.ExecutePostAsync(request);
    }
}