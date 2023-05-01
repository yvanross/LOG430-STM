﻿using System.Threading.Channels;
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

    public static Task<ChannelReader<RestResponse>?> Get(GetRoutingRequest routingRequest)
    {
        return Try.WithConsequenceAsync(() => RestUc.Get(routingRequest), retryCount: 3, autoThrow: false)!;
    }

    public static Task<ChannelReader<RestResponse>?> Post<T>(PostRoutingRequest<T> routingRequest) where T : class
    {
        return Try.WithConsequenceAsync(() => RestUc.Post(routingRequest), retryCount: 3, autoThrow: false)!;
    }

    public static Task<IEnumerable<RoutingData>> GetAddress(string targetService, LoadBalancingMode mode)
    {
        return Try.WithConsequenceAsync(() => RestUc.GetServiceRoutingData(targetService, mode), retryCount: 3, autoThrow: false)!;
    }
}