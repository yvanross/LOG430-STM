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

    private readonly SingleTokenAdder _tokenAdder;

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

    protected L4Link(ITunnel source, ITunnel destination, SingleTokenAdder tokenAdder)
    {
        Source = source;
        Destination = destination;
        _tokenAdder = tokenAdder;
    }

    protected async Task SafeAbortGossips()
    {
        if (CancellationTokenSource.IsCancellationRequested is false)
        {
            CancellationTokenSource.Cancel();

            await _tokenAdder.Take(new CancellationTokenSource(500).Token);

            _tokenAdder.Release();
        }
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

    private async Task<LinkResult> TaskRelease(byte[] bufferArray)
    {
        var result = await TryCopyDataAsync(bufferArray);

        _tokenAdder.Release();

        return result;
    }

    private protected abstract Task<LinkResult> TryCopyDataAsync(byte[] bufferArray);


    public void Dispose()
    {
        SafeAbortGossips().Wait();

        Source.Dispose();

        Destination.Dispose();

        CancellationTokenSource.Dispose();

        if(GossipingTask is not null)
            Interlocked.Exchange(ref GossipingTask, null);

        GossipingTask?.Dispose();
    }
}