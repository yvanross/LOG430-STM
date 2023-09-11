using System.Net.Sockets;
using System.Threading.Tasks;
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

    private readonly CancellationTokenSource _blueReadCancellationTokenSource = new ();
    private readonly CancellationTokenSource _greenReadCancellationTokenSource = new ();
    private readonly CancellationTokenSource _blueWriteCancellationTokenSource = new ();
    private readonly CancellationTokenSource _greenWriteCancellationTokenSource = new ();

    private const int NumberOfLinks = 4;

    private int GreenPort { get; set; }

    //blue -> green
    public LinkHub(ConnectionContext sourceConnection, IServiceType serviceType, IRouting routing,
        IPodReadService podReadService, ILogger logger, CancellationToken connectionConnectionClosed)
    {
        _serviceType = serviceType;
        _routing = routing;
        _podReadService = podReadService;
        _logger = logger;

        connectionConnectionClosed.Register(() =>
        {
            TryCancelToken(_cancellationTokenSource);
        });

        _cancellationTokenSource.Token.Register(() =>
        {
            TryCancelToken(_blueWriteCancellationTokenSource);
            TryCancelToken(_blueReadCancellationTokenSource);
            TryCancelToken(_greenWriteCancellationTokenSource);
            TryCancelToken(_greenReadCancellationTokenSource);
        });


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
            await ScheduleLinks();
        }
        catch (BlueLinkException e)
        {
            _logger.LogError(e, "Blue link exception");
        }
    }

    private async Task ScheduleLinks()
    {
        _logger.LogInformation("Starting link hub");

        var tasks = new[]
        {
            (nameof(BlueRead), _blueReadLink.BeginGossiping(_blueReadCancellationTokenSource.Token).ContinueWith(t =>
            {
                try
                {
                    CheckForException(t);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Blue read link exception");
                }
                finally
                {
                    TryCancelToken(_greenReadCancellationTokenSource);
                }

                return t;
            })),
            (nameof(BlueWrite), _blueWriteLink.BeginGossiping(_blueWriteCancellationTokenSource.Token).ContinueWith(t =>
            {
                try
                {
                    CheckForException(t);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Blue write link exception");
                }
                finally
                {
                    //TryCancelToken(_greenReadCancellationTokenSource);
                }

                return t;
            })),
            (nameof(GreenWrite), _greenWriteLink.BeginGossiping(_greenWriteCancellationTokenSource.Token).ContinueWith(t =>
            {
                try
                {
                    CheckForException(t);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Green write link exception");
                }
                finally
                {
                    //TryCancelToken(_blueWriteCancellationTokenSource);
                }

                return t;
            })),
            (nameof(GreenRead), _greenReadLink.BeginGossiping(_greenReadCancellationTokenSource.Token).ContinueWith(t =>
            {
                try
                {
                    CheckForException(t);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Green read link exception");
                }
                finally
                {
                    NotifyRoutingOfUnresponsiveService(GreenPort);
                    //TryCancelToken(_blueWriteCancellationTokenSource);
                }

                return t;
            }))
        };

        _logger.LogInformation("Chatting...");

        var finishedTask = await Task.WhenAny(tasks.Select(kv=>kv.Item2));

        _logger.LogInformation($"Conversation over, {tasks.Single(t => t.Item2.Id.Equals(finishedTask.Id)).Item1} completed.");
    }



    private static void CheckForException(Task task)
    {
        if (task.Status == TaskStatus.Faulted) throw task.Exception.InnerException;

        if (task is Task<Task> nestedTaskContainer)
        {
            var nestedTask = nestedTaskContainer.Unwrap();

            CheckForException(nestedTask);
        }
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
            GreenPort = int.Parse(destination.Port);

            if (_podReadService.GetAllServices()
                    .FirstOrDefault(si =>
                        si.ContainerInfo is not null &&
                        si.ContainerInfo.PortsInfo.RoutingPortNumber.Equals(GreenPort)) is
                { } serviceInstance)
            {
                _logger.LogInformation($"Routing to {serviceInstance.ContainerInfo.Name}");
            }

            try
            {
                var destinationClient = new TcpClient(destination.Host, GreenPort);

                _logger.LogInformation($"Connected on {GreenPort}");

                return destinationClient;
            }
            catch (Exception)
            {
                NotifyRoutingOfUnresponsiveService(GreenPort);

                throw new TcpConnectionException("TCP connection failed");
            }
        }

        throw new TcpConnectionException("No possible TCP connections");
    }

    private void TryCancelToken(CancellationTokenSource cancellationTokenSource)
    {
        if (cancellationTokenSource.IsCancellationRequested is false)
            cancellationTokenSource.Cancel();
    }
}