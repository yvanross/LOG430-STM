using System.Net.Sockets;
using System.Text;
using System.Threading;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.States;
using Entities.Dao;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Infrastructure.L4ConnectionListener.Exceptions;
using Infrastructure.L4ConnectionListener.L4LinkBuffers;
using Infrastructure.L4ConnectionListener.Links;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Polly;

namespace Infrastructure.L4ConnectionListener;

public sealed class LinkHub : IDisposable
{
    private readonly IServiceType _serviceType;
    private readonly IRouting _routing;
    private readonly IPodReadService _podReadService;
    private readonly ILogger _logger;

    private readonly BlueWrite _blueWriteLink;
    private readonly BlueRead _blueReadLink;

    private readonly GreenWrite _greenWriteLink;
    private readonly GreenRead _greenReadLink;

    private readonly ChannelTunnel _blueStream = new();
    private readonly ChannelTunnel _greenStream = new();

    private readonly SemaphoreSlim _semaphoreSlim = new (4);
    private readonly CancellationTokenSource _cancellationTokenSource = new (); 

    public LinkHub(ConnectionContext sourceConnection, IServiceType serviceType, IRouting routing, IPodReadService podReadService, ILogger logger)
    {
        _serviceType = serviceType;
        _routing = routing;
        _podReadService = podReadService;
        _logger = logger;

        var destinationStream = GetTcpClient().GetStream();

        _blueWriteLink = new BlueWrite(new StreamTunnel(sourceConnection.Transport.Input.AsStream()), _blueStream, logger, new SingleTokenAdder(_semaphoreSlim, logger));
        _blueReadLink = new BlueRead(_greenStream,  new StreamTunnel(sourceConnection.Transport.Output.AsStream()), logger, new SingleTokenAdder(_semaphoreSlim, logger));

        _greenWriteLink = new GreenWrite(_blueStream, new StreamTunnel(destinationStream), logger, new SingleTokenAdder(_semaphoreSlim, logger));
        _greenReadLink = new GreenRead(new StreamTunnel(destinationStream), _greenStream, logger, new SingleTokenAdder(_semaphoreSlim, logger));
    }

    public async Task BeginAsync()
    {
        try
        {
            var WriteFinishFirst = false;

            while (true)
            {
                _logger.LogInformation("Starting link hub");

                var blueRead = await _blueReadLink.BeginGossiping(_cancellationTokenSource.Token);
                var blueWrite = await _blueWriteLink.BeginGossiping(_cancellationTokenSource.Token);

                var greenWrite = await _greenWriteLink.BeginGossiping(_cancellationTokenSource.Token);
                var greenRead = await _greenReadLink.BeginGossiping(_cancellationTokenSource.Token);

                _logger.LogInformation("Connection established...");

                var finishedTask = await Task.WhenAny(blueRead, blueWrite, greenRead, greenWrite);

                if (finishedTask == greenRead && greenRead.Result.Equals(LinkResult.Retry))
                {
                    UpdateDestination();

                    continue;
                }
                if (finishedTask == greenWrite && greenWrite.Result.Equals(LinkResult.Retry))
                {
                    UpdateDestination();

                    continue;
                }

                _logger.LogInformation("Link hub closing");

                break;
            }
        }
        catch (BlueLinkException e)
        {
            _logger.LogError(e, "Blue link exception");
        }
    }

    private void UpdateDestination()
    {
        var destinationClient = GetTcpClient();

        _logger.LogInformation($"Updating Green Link host...");

        Task.WaitAll(
            _greenReadLink.UpdateSource(new StreamTunnel(destinationClient.GetStream())),
            _greenWriteLink.UpdateDestination(new StreamTunnel(destinationClient.GetStream())));

        _logger.LogInformation($"Update Completed, ready");
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing link hub");

        if (!_cancellationTokenSource.IsCancellationRequested)
            _cancellationTokenSource.Cancel();

        _cancellationTokenSource.Dispose();
        _blueWriteLink.Dispose();
        _blueReadLink.Dispose();
        _greenWriteLink.Dispose();
        _greenReadLink.Dispose();
        _blueStream.Dispose();
        _greenStream.Dispose();
    }

    private TcpClient GetTcpClient()
    {
        var backOffRetry = Policy.Handle<TcpConnectionException>()
            .WaitAndRetry(10, attempt => TimeSpan.FromSeconds(Math.Max(attempt/2, 5)));

        return backOffRetry.Execute(() => GetDestinationClient(_serviceType));
    }

    private void NotifyRoutingOfUnresponsiveService(int port)
    {
        var unresponsiveService = _podReadService.GetAllServices().FirstOrDefault(s =>
            (s.ContainerInfo?.PortsInfo.RoutingPortNumber.Equals(port) ?? false) &&
            s.ServiceStatus is ReadyState);

        if (unresponsiveService != null)
            _routing.RegisterUnresponsive(unresponsiveService);
    }

    private TcpClient GetDestinationClient(IServiceType serviceType)
    {
        var destination = _routing.RouteByDestinationType("$*$", serviceType.Type, LoadBalancingMode.RoundRobin).FirstOrDefault();

        if (destination is not null)
        {
            var port = int.Parse(destination.Port);

            try
            {
                var destinationClient = new TcpClient(destination.Host, port);

                _logger.LogInformation($"Connected on {port}");

                destinationClient.SendTimeout = 1000;
                destinationClient.ReceiveTimeout = 1000;

                return destinationClient;
            }
            catch (Exception e)
            {
                NotifyRoutingOfUnresponsiveService(port);

                throw new TcpConnectionException("TCP connection failed");
            }
        }

        throw new TcpConnectionException("No possible TCP connections");
    }
}