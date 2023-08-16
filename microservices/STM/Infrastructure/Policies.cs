using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Infrastructure2;

public static class Policies
{
    public static AsyncRetryPolicy ClientExecution(ILogger logger)
        => Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(retryAttempt / 5.0),
                (exception, _) => logger.LogCritical(exception.ToString()));

    public static AsyncRetryPolicy InfiniteAsyncRetry(ILogger logger)
        => Policy
            .Handle<Exception>()
            .RetryForeverAsync(exception => logger.LogCritical(exception.ToString()));
}