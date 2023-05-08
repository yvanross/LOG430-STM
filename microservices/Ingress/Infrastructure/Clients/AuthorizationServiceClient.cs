using ApplicationLogic.Interfaces;
using Docker.DotNet.Models;
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
            Role = "User",
            Secret = secret
        });

        var tokenRes = await _client.ExecutePostAsync<string?>(req);

        tokenRes.ThrowIfError();

        return tokenRes.Data;
    }

    public async Task Register(string name, string secret, string teamName, string group)
    {
        var req = new RestRequest($"Register/{group}/{teamName}/{name}", Method.Post);

        req.AddJsonBody(new
        {
            Role = "User",
            Secret = secret
        });

        var role = await _client.ExecutePostAsync(req);

        role.ThrowIfError();
    }

    public async Task<string[]> GetVisibleAccounts(string jwt)
    {
        var req = new RestRequest($"User");

        req.AddHeader("Authorization", $"Bearer {jwt}");

        var tokenRes = await _client.ExecuteGetAsync<string[]>(req);

        tokenRes.ThrowIfError();

        return tokenRes.Data;
    }

    public async Task Remove(string nodeName)
    {
        var req = new RestRequest($"User/{nodeName}");

        var res = await _client.DeleteAsync(req);

        res.ThrowIfError();
    }
}