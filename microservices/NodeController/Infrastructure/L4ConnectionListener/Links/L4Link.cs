using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading;
using Entities.Extensions;
using Infrastructure.L4ConnectionListener.L4LinkBuffers;

namespace Infrastructure.L4ConnectionListener.Links;

public abstract class L4Link : IDisposable
{
    public ITunnel Source
    {
        get => _source;
        private protected set => _source = value;
    }

    public ITunnel Destination
    {
        get => _destination;
        private protected set => _destination = value;
    }

    private protected ITunnel _source;

    private protected ITunnel _destination;

    private readonly ILogger _logger;

    private readonly SingleTokenAdder _tokenAdder;

    private protected string WhoAmI;

    private protected readonly ConcurrentQueue<byte[]> FailoverBufferQueue = new();

    private protected CancellationTokenSource CancellationTokenSource = new();

    private protected Task<LinkResult>? GossipingTask;

    private protected readonly SocketError[] SocketErrorsToRetry =
    {
        SocketError.ConnectionReset,
        SocketError.ConnectionAborted,
        SocketError.OperationAborted,
        SocketError.Interrupted,
        SocketError.Shutdown,
        SocketError.NotConnected,
        SocketError.TimedOut,
        SocketError.HostDown,
        SocketError.HostUnreachable,
        SocketError.NetworkDown,
        SocketError.NetworkUnreachable,
        SocketError.ConnectionRefused,
        SocketError.MessageSize
    };

    protected L4Link(ITunnel source, ITunnel destination, ILogger logger, SingleTokenAdder tokenAdder)
    {
        Source = source;
        Destination = destination;
        _logger = logger;
        _tokenAdder = tokenAdder;
    }

    public virtual async Task SafeAbortGossips()
    {
        if (CancellationTokenSource.IsCancellationRequested is false)
        {
            CancellationTokenSource.Cancel();

            await _tokenAdder.Take(CancellationToken.None);

            _tokenAdder.Release();

            Interlocked.Exchange(ref CancellationTokenSource, new CancellationTokenSource());
        }

        if (Interlocked.Exchange(ref GossipingTask, null) is {} task)
            await task;
    }

    public async Task<Task<LinkResult>> BeginGossiping(CancellationToken cancellation)
    {
        if (GossipingTask is null)
        {
            await _tokenAdder.Take(cancellation);

            byte[] bufferArray = new byte[8_192];

            return GossipingTask ??= TaskRelease(bufferArray);
        }

        return GossipingTask;
    }


    public async Task ResendPossiblyLostDataToDestination()
    {
        while (FailoverBufferQueue.Count > 0 && FailoverBufferQueue.TryPeek(out var dataChunk))
        {
            _logger.LogInformation($"Compensating last messages on {WhoAmI}");

            //await Destination.WriteAsync(dataChunk, CancellationToken.None);

            FailoverBufferQueue.TryDequeue(out _);
        }
    }

    private async Task<LinkResult> TaskRelease(byte[] bufferArray)
    {
        var result = await TryCopyDataAsync(bufferArray);

        _tokenAdder.Release();

        return result;
    }

    private protected abstract Task<LinkResult> TryCopyDataAsync(byte[] bufferArray);


    public void Dispose()
    {
        Source.Dispose();
        Destination.Dispose();

        if (CancellationTokenSource.IsCancellationRequested is false)
            CancellationTokenSource.Cancel();

        CancellationTokenSource.Dispose();

        Interlocked.Exchange(ref GossipingTask, null);
    }
}