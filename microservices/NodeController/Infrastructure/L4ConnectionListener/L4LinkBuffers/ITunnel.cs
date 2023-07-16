namespace Infrastructure.L4ConnectionListener.L4LinkBuffers;

public interface ITunnel : IDisposable
{
    ValueTask<int> ReadAsync(byte[] buffer, CancellationToken cancellationToken);

    public ValueTask WriteAsync(byte[] buffer, CancellationToken cancellationToken);
}