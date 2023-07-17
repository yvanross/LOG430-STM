using System.Threading.Channels;
using Infrastructure.L4ConnectionListener.L4LinkBuffers;

public class ChannelTunnel : ITunnel
{
    private readonly Channel<byte[]> _channel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = true,
    });

    private MemoryStream _memoryStream = new ();

    public void Dispose()
    {
        _channel.Writer.TryComplete();
        _memoryStream.Dispose();
    }

    public async ValueTask<int> ReadAsync(byte[] buffer, CancellationToken cancellationToken)
    {
        if (_memoryStream.Length == _memoryStream.Position) // If we have read everything from the MemoryStream
        {
            if (!await RefillBufferAsync(cancellationToken)) // If we can't read more from the Channel
            {
                return 0; // Signal the end of the stream
            }
        }

        // Read from the MemoryStream
        return await _memoryStream.ReadAsync(buffer, cancellationToken);
    }

    private async ValueTask<bool> RefillBufferAsync(CancellationToken cancellationToken)
    {
        byte[] bytesFromChannel;
        try
        {
            if (!_channel.Reader.TryRead(out bytesFromChannel))
            {
                bytesFromChannel = await _channel.Reader.ReadAsync(cancellationToken);
            }
        }
        catch (ChannelClosedException)
        {
            return false; // Signal the end of the stream
        }

        _memoryStream = new MemoryStream(bytesFromChannel);

        return true;
    }

    public async ValueTask WriteAsync(byte[] buffer, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(buffer, cancellationToken);
    }
}