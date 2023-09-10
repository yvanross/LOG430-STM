using Microsoft.Extensions.Logging;

namespace Infrastructure.L4ConnectionListener;

public class SingleTokenAdder
{
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly SemaphoreSlim _mutex = new (1);
    private readonly ILogger _logger;

    public SingleTokenAdder(SemaphoreSlim semaphoreSlim, ILogger logger)
    {
        _semaphoreSlim = semaphoreSlim;
        _logger = logger;
    }

    public void Release()
    {
        if (_mutex.CurrentCount < 1)
        {
            _semaphoreSlim.Release();
        }
    }

    public Task Take(CancellationToken cancellation)
    {
        try
        {
            _mutex.WaitAsync(TimeSpan.FromMilliseconds(500), cancellation);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to acquire mutex");
            throw;
        }
            
        return _semaphoreSlim.WaitAsync(cancellation);
    }
}