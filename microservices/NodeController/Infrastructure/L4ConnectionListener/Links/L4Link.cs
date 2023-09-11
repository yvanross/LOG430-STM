using Infrastructure.L4ConnectionListener.L4LinkBuffers;

namespace Infrastructure.L4ConnectionListener.Links;

public abstract class L4Link : IDisposable
{
    protected ITunnel Source { get; }

    protected ITunnel Destination { get; }

    private readonly SingleTokenAdder _tokenAdder;

    private protected readonly CancellationTokenSource CancellationTokenSource = new ();

    private Task? _gossipingTask;

    protected L4Link(ITunnel source, ITunnel destination, SingleTokenAdder tokenAdder)
    {
        Source = source;
        Destination = destination;
        _tokenAdder = tokenAdder;
    }

    public async Task BeginGossiping(CancellationToken cancellation)
    {
        if (_gossipingTask is null)
        {
            await _tokenAdder.Take(cancellation);

            byte[] bufferArray = new byte[8_192];

            _gossipingTask ??= TaskRelease(bufferArray);
        }

        await _gossipingTask;
    }

    public void Dispose()
    {
        SafeAbortGossips().Wait();

        Source.Dispose();

        Destination.Dispose();

        CancellationTokenSource.Dispose();

        if (_gossipingTask is not null)
            Interlocked.Exchange(ref _gossipingTask, null);

        _gossipingTask?.Dispose();
    }


    private async Task TaskRelease(byte[] bufferArray)
    {
        await TryCopyDataAsync(bufferArray);

        _tokenAdder.Release();
    }

    private protected abstract Task TryCopyDataAsync(byte[] bufferArray);

    private async Task SafeAbortGossips()
    {
        if (CancellationTokenSource.IsCancellationRequested is false)
        {
            CancellationTokenSource.Cancel();

            await _tokenAdder.Take(new CancellationTokenSource(500).Token);

            _tokenAdder.Release();
        }
    }
}