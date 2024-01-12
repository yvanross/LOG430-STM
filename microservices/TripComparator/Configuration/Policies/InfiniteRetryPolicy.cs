using Application.Interfaces.Policies;
using Polly;
using Polly.Retry;

namespace Configuration.Policies;

public class InfiniteRetryPolicy<TClass> : IInfiniteRetryPolicy<TClass> where TClass : class
{
    private readonly AsyncRetryPolicy _asyncRetryPolicy;

    public InfiniteRetryPolicy(ILogger<TClass> logger)
    {
        _asyncRetryPolicy = Policy
            .Handle<Exception>()
            .RetryForeverAsync(exception => logger.LogCritical(exception.ToString()));
    }

    public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
    {
        return _asyncRetryPolicy.ExecuteAsync(action);
    }

    public Task ExecuteAsync(Func<Task> action)
    {
        return _asyncRetryPolicy.ExecuteAsync(action);
    }
}