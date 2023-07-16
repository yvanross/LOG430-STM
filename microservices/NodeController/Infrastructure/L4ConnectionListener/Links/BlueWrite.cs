using Infrastructure.L4ConnectionListener.L4LinkBuffers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.L4ConnectionListener.Links;

public class BlueWrite : L4Link
{
    public BlueWrite(ITunnel source, ITunnel destination, ILogger logger, SingleTokenAdder tokenAdder) : base(source, destination, logger, tokenAdder)
    {
        WhoAmI = nameof(BlueWrite);
    }

     private protected override async Task<LinkResult> TryCopyDataAsync(byte[] bufferArray)
    {
        int bytesRead;

        try
        {
            while ((bytesRead = await Source.ReadAsync(bufferArray, CancellationToken.None)) > 0)
            {
                var dataChunk = new byte[bytesRead];

                Buffer.BlockCopy(bufferArray, 0, dataChunk, 0, bytesRead);

                // Add the data chunk to the queue.
                FailoverBufferQueue.Enqueue(dataChunk);

                CancellationTokenSource.Token.ThrowIfCancellationRequested();

                // Write the data chunk to the destination stream.
                await Destination.WriteAsync(dataChunk, CancellationTokenSource.Token);

                // Remove the data chunk from the queue, as it has been successfully written to the destination.
                FailoverBufferQueue.TryDequeue(out _);
            }
        }
        catch (Exception e)
        {
            return LinkResult.Retry;
        }
        finally
        {
            Logger.LogInformation($"{nameof(BlueWrite)} Exiting");
        }

        return LinkResult.Abort;
    }
}