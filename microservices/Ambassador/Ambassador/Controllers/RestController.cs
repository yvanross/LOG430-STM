using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador.BusinessObjects;
using Ambassador.Extensions;
using Ambassador.Usecases;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Ambassador.Controllers;

public static class RestController
{
    private static readonly RestUC RestUc = new ();

    public static async Task<RestResponse> Get(GetRoutingRequest routingRequest)
    {
        return await Try.WithConsequenceAsync(async () => await RestUc.Get(routingRequest), retryCount: 3);
    }

    public static async Task<RestResponse> Post<T>(PostRoutingRequest<T> routingRequest) where T : class
    {
        return await Try.WithConsequenceAsync(async () => await RestUc.Post(routingRequest), retryCount: 3);
    }
}