using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;

namespace Infrastructure;

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