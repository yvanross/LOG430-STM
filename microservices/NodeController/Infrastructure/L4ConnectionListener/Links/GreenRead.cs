using Entities.Extensions;
using Infrastructure.L4ConnectionListener.L4LinkBuffers;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace Infrastructure.L4ConnectionListener.Links;

public class GreenRead : L4Link
{
    public GreenRead(ITunnel source, ITunnel destination, ILogger logger, SingleTokenAdder tokenAdder) : base(source, destination, logger, tokenAdder)
    {
        WhoAmI = nameof(GreenRead);
    }

    private protected override async Task<LinkResult> TryCopyDataAsync(byte[] bufferArray)
    {
        int bytesRead;
        try
        {
            while ((bytesRead = await Source.ReadAsync(bufferArray, CancellationTokenSource.Token)) > 0)
            {
                var dataChunk = new byte[bytesRead];

                Buffer.BlockCopy(bufferArray, 0, dataChunk, 0, bytesRead);

                // Add the data chunk to the queue.
                FailoverBufferQueue.Enqueue(dataChunk);

                // Write the data chunk to the destination stream.
                await Destination.WriteAsync(dataChunk, CancellationTokenSource.Token);

                // Remove the data chunk from the queue, as it has been successfully written to the destination.
                FailoverBufferQueue.TryDequeue(out _);
            }
        }
        catch (Exception e) when (
            e is OperationCanceledException ||
            e is IOException &&
            SocketErrorsToRetry.Any(error => error.Equals((e as IOException)!.InnerException?.HResult)) ||
            e is SocketException &&
            SocketErrorsToRetry.Any(error => error.Equals((e as SocketException)!.SocketErrorCode)))
        {
            return LinkResult.Retry;
        }
        finally
        {
            Logger.LogInformation($"{nameof(GreenRead)} Exiting");
        }

        return LinkResult.Retry;
    }

    public async Task UpdateSource(ITunnel source)
    {
        await SafeAbortConnection();

        if (GossipingTask != null) await GossipingTask;

        Interlocked.Exchange(ref Source, source);

        Interlocked.Exchange(ref CancellationTokenSource, new CancellationTokenSource());
    }

    private protected override async Task SafeAbortConnection()
    {
        await base.SafeAbortConnection();

        Source.Dispose();
    }
}