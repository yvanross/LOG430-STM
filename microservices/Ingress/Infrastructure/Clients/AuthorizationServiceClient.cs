using ApplicationLogic.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.Clients;

public class AuthorizationServiceClient : IAuthorizationService
{
    private readonly RestClient _client;

    private readonly ILogger _logger;

    public AuthorizationServiceClient(ILogger<AuthorizationServiceClient> logger, IHostInfo hostInfo)
    {
        _logger = logger;
        _client = new RestClient($"http://{hostInfo.GetAddress()}:{hostInfo.GetAuthServicePort()}");
    }

    public async Task<string?> Authorize(string username, string secret)
    {
        var req = new RestRequest($"Authorize/{username}", Method.Post);

        req.AddJsonBody(new
        {
            secret
        });

        var role = await _client.ExecutePostAsync<string?>(req);

        role.ThrowIfError();

        return role.Data;
    }

    public async Task Register(string name, string secret, string teamName)
    {
        var req = new RestRequest($"Register/{teamName}/{name}", Method.Post);

        req.AddJsonBody(new
        {
            secret
        });

        var role = await _client.ExecutePostAsync(req);

        role.ThrowIfError();
    }
}