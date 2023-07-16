using Microsoft.Extensions.Logging;
using System.Threading;

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
            _logger.LogInformation("releasing semaphore");

            _semaphoreSlim.Release();
        }
    }

    public Task Take(CancellationToken cancellation)
    {
        _mutex.WaitAsync(cancellation);
            
        _logger.LogInformation($"taking semaphore, {_semaphoreSlim.CurrentCount} left");

        return _semaphoreSlim.WaitAsync(cancellation);
    }
}