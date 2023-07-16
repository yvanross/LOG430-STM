using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading;
using Entities.Extensions;
using Infrastructure.L4ConnectionListener.L4LinkBuffers;

namespace Infrastructure.L4ConnectionListener.Links;

public abstract class L4Link : IDisposable
{
    private protected ITunnel Source;
    private protected ITunnel Destination;
    private protected readonly ILogger Logger;
    private protected readonly SingleTokenAdder TokenAdder;
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
        Logger = logger;
        TokenAdder = tokenAdder;
    }

    private protected virtual async Task SafeAbortConnection()
    {
        Logger.LogInformation($"aborting gossips on {WhoAmI}");

        if (CancellationTokenSource.IsCancellationRequested is false)
        {
            CancellationTokenSource.Cancel();

            await TokenAdder.Take(CancellationToken.None);

            TokenAdder.Release();
        }

        if(Interlocked.Exchange(ref GossipingTask, null) is {} task)
            await task;
    }

    public async Task<Task<LinkResult>> BeginGossiping(CancellationToken cancellation)
    {
        if (GossipingTask is null)
        {
            await TokenAdder.Take(cancellation);

            byte[] bufferArray = new byte[8];

            return GossipingTask ??= TaskRelease(bufferArray);
        }

        return GossipingTask;
    }

    private async Task<LinkResult> TaskRelease(byte[] bufferArray)
    {
        var result = await TryCopyDataAsync(bufferArray);

        TokenAdder.Release();

        return result;
    }

    private protected abstract Task<LinkResult> TryCopyDataAsync(byte[] bufferArray);

    //public async Task ResendPossiblyLostDataToDestination()
    //{
    //    while (FailoverBufferQueue.Count > 0 && FailoverBufferQueue.TryPeek(out var dataChunk))
    //    {
    //        Logger.LogInformation($"Compensating last messages on {WhoAmI}");

    //        await Destination.WriteAsync(dataChunk, CancellationToken.None);

    //        FailoverBufferQueue.TryDequeue(out _);
    //    }
    //}

    public void Dispose()
    {
        Logger.LogInformation($"Disposing link {WhoAmI}");

        Source.Dispose();
        Destination.Dispose();

        if (CancellationTokenSource.IsCancellationRequested is false)
            CancellationTokenSource.Cancel();

        CancellationTokenSource.Dispose();

        Interlocked.Exchange(ref GossipingTask, null);
    }
}