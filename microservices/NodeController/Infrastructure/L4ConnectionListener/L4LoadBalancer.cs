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

    private static bool _firstBoot = true;
    private static DateTime _logLock = DateTime.MinValue;

    public L4LoadBalancer(IRouting routing, IPodReadService podReadService, L4Logger logger)
    {
        _routing = routing;
        _podReadService = podReadService;
        _logger = logger;

        if (_firstBoot)
        {
            _firstBoot = false;
            _logLock = DateTime.UtcNow + TimeSpan.FromSeconds(15);
        }

        _logger.Lock(_logLock);
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        try
        {
            _logger.SetConnectionId(connection.ConnectionId);

            _logger.LogInformation($"New connection received: {connection.ConnectionId}");

            var incomingPort = ((IPEndPoint)connection.LocalEndPoint!)!.Port;

            if (_podReadService.TryGetServiceTypeFromPort(incomingPort) is not { } serviceType)
                throw new Exception("Port number not resolved to target service type. Call NegotiateSocket from Routing/ first to create or get the right socket");

            using var linkHub = new LinkHub(connection, serviceType, _routing, _podReadService, _logger, connection.ConnectionClosed);

            await linkHub.BeginAsync();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
        }
        finally
        {
            connection.Abort();

            _ = connection.DisposeAsync();

            _logger.LogInformation($"Connection disposed: {connection.ConnectionId}");
        }
    }
}