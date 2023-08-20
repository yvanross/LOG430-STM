﻿using Infrastructure.L4ConnectionListener.Exceptions;
using Infrastructure.L4ConnectionListener.L4LinkBuffers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.L4ConnectionListener.Links;

public class BlueWrite : L4Link
{
    public BlueWrite(ITunnel source, ITunnel destination, SingleTokenAdder tokenAdder) : base(source, destination, tokenAdder) {}

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
        catch
        {
            throw new BlueLinkException("Write Closed");
        }

        return LinkResult.Abort;
    }
}