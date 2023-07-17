using Infrastructure.L4ConnectionListener.L4LinkBuffers;
using Microsoft.Extensions.Logging;
using Infrastructure.L4ConnectionListener.Exceptions;

namespace Infrastructure.L4ConnectionListener.Links;

public class BlueRead : L4Link
{
    public BlueRead(ITunnel source, ITunnel destination, ILogger logger, SingleTokenAdder tokenAdder) : base(source, destination, logger, tokenAdder)
    {
        WhoAmI = nameof(BlueRead);
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

                // Write the data chunk to the destination stream.
                await Destination.WriteAsync(dataChunk, CancellationTokenSource.Token);
            }
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return LinkResult.Retry;
        }
        catch
        {
            throw new BlueLinkException("Read Closed");
        }

        return LinkResult.Abort;
    }
}