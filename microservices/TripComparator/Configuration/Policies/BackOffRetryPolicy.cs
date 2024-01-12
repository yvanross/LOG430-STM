using Application.Interfaces.Policies;
using Polly;
using Polly.Retry;

namespace Configuration.Policies;

public class BackOffRetryPolicy<TClass> : IBackOffRetryPolicy<TClass> where TClass : class
{
    private readonly AsyncRetryPolicy _asyncRetryPolicy;

    public BackOffRetryPolicy(ILogger<TClass> logger)
    {
        _asyncRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(retryAttempt / 5.0),
                (exception, _) => logger.LogCritical(exception.ToString()));
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