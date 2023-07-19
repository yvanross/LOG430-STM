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

    private readonly SemaphoreSlim _semaphoreSlim = new (NumberOfLinks);
    private readonly CancellationTokenSource _cancellationTokenSource = new ();

    private const int NumberOfLinks = 4;

    public LinkHub(ConnectionContext sourceConnection, IServiceType serviceType, IRouting routing, IPodReadService podReadService, ILogger logger)
    {
        _serviceType = serviceType;
        _routing = routing;
        _podReadService = podReadService;
        _logger = logger;

        var destinationStream = GetTcpClient().GetStream();

        _blueWriteLink = new BlueWrite(new StreamTunnel(sourceConnection.Transport.Input.AsStream()), _blueStream, new SingleTokenAdder(_semaphoreSlim, logger));
        _blueReadLink = new BlueRead(_greenStream,  new StreamTunnel(sourceConnection.Transport.Output.AsStream()), new SingleTokenAdder(_semaphoreSlim, logger));

        _greenWriteLink = new GreenWrite(_blueStream, new StreamTunnel(destinationStream), new SingleTokenAdder(_semaphoreSlim, logger));
        _greenReadLink = new GreenRead(new StreamTunnel(destinationStream), _greenStream, new SingleTokenAdder(_semaphoreSlim, logger));
    }

    public async Task BeginAsync()
    {
        try
        {
            while ((await ScheduleLinks()).Equals(LinkResult.Retry)) { }
        }
        catch (BlueLinkException e)
        {
            _logger.LogError(e, "Blue link exception");
        }
    }

    private async Task<LinkResult> ScheduleLinks()
    {
        _logger.LogInformation("Starting link hub");

        var tasks = new[]
        {
            (nameof(BlueRead), await _blueReadLink.BeginGossiping(_cancellationTokenSource.Token)),
            (nameof(BlueWrite), await _blueWriteLink.BeginGossiping(_cancellationTokenSource.Token)),

            (nameof(GreenWrite), await _greenWriteLink.BeginGossiping(_cancellationTokenSource.Token)),
            (nameof(GreenRead), await _greenReadLink.BeginGossiping(_cancellationTokenSource.Token)),
        };

        _logger.LogInformation("Chatting...");

        var finishedTask = await Task.WhenAny(tasks.Select(kv=>kv.Item2));

        _logger.LogInformation($"Conversation over, {tasks.Single(t => t.Item2.Id.Equals(finishedTask.Id)).Item1} completed.");

        return await finishedTask;
    }

    public void Dispose()
    {
        try
        {
            _logger.LogInformation("Disposing link hub");

            if (!_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel();

            _cancellationTokenSource.Dispose();

            Task.WaitAll(new[]
            {
                new Task(() => _blueWriteLink.Dispose()),
                new Task(() => _greenWriteLink.Dispose()),
                new Task(() => _greenReadLink.Dispose()),
                new Task(() => _blueReadLink.Dispose()),
                new Task(() => _blueStream.Dispose()),
                new Task(() => _greenStream.Dispose()),
                _semaphoreSlim.WaitAsync(),
                _semaphoreSlim.WaitAsync(),
                _semaphoreSlim.WaitAsync(),
                _semaphoreSlim.WaitAsync()
            }, TimeSpan.FromSeconds(1));

            _semaphoreSlim.Dispose();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error disposing link hub");
        }
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