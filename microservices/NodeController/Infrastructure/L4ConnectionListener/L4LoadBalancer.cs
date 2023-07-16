using System.Collections.Concurrent;
using ApplicationLogic.Interfaces;
using System.Net;
using Entities.Dao;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Infrastructure.L4ConnectionListener;

public class L4LoadBalancer : ConnectionHandler
{
    private readonly IRouting _routing;
    private readonly IPodReadService _podReadService;
    private readonly L4Logger _logger;

    public L4LoadBalancer(IRouting routing, IPodReadService podReadService, L4Logger logger)
    {
        _routing = routing;
        _podReadService = podReadService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        try
        {
            var incomingPort = ((IPEndPoint)connection.LocalEndPoint!)!.Port;

            if (_podReadService.TryGetServiceTypeFromPort(incomingPort) is not { } serviceType)
                throw new Exception("Port number not resolved to target service type. Call NegotiateSocket from Routing/ first to create or get the right socket");

            _logger.SetConnectionId(connection.ConnectionId);

            using var linkHub = new LinkHub(connection, serviceType, _routing, _podReadService, _logger);

            await linkHub.BeginAsync();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
}