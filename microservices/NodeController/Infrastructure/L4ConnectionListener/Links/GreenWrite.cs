using Infrastructure.L4ConnectionListener.L4LinkBuffers;
using System.Net.Sockets;

namespace Infrastructure.L4ConnectionListener.Links;

/// <summary>
///   This link is responsible for reading data from the hub and writing it to the destination service.
/// </summary>
public class GreenWrite : L4Link
{
    public GreenWrite(ITunnel source, ITunnel destination, SingleTokenAdder tokenAdder) : base(source, destination, tokenAdder)
    {
    }

    private protected override async Task TryCopyDataAsync(byte[] bufferArray)
    {
        int bytesRead;

        while ((bytesRead = await Source.ReadAsync(bufferArray, CancellationTokenSource.Token)) > 0)
        {
            var dataChunk = new byte[bytesRead];
            Buffer.BlockCopy(bufferArray, 0, dataChunk, 0, bytesRead);
            // Write the data chunk to the destination stream.
            await Destination.WriteAsync(dataChunk, CancellationTokenSource.Token);
        }
    }
}