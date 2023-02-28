namespace Ingress.Extensions;

public static class Try
{

    public static async Task WithConsequenceAsync(Func<Task> todo, Action<Exception, int>? onFailure = null, int retryCount = 0)
        => await WithConsequenceAsync(async () =>
        {
            await todo();
            return Task.CompletedTask;
        }, onFailure, retryCount);

    public static async Task<T> WithConsequenceAsync<T>(Func<Task<T>> todo, Action<Exception, int>? onFailure = null, int retryCount = 0)
    {
        var retry = 0;

        return await SafeAction(todo, onFailure);

        async Task<T> SafeAction(Func<Task<T>> func, Action<Exception, int>? action)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (retry < retryCount)
                {
                    retry++;
                    return await SafeAction(func, action);
                }

                if (action is { } next) next(e, retry);

                throw;
            }
        }
    }

    public static T WithConsequence<T>(Func<T> todo, Action<Exception, int>? onFailure = null, int retryCount = 0)
    {
        var retry = 0;

        return SafeAction(todo, onFailure);

        T SafeAction(Func<T> func, Action<Exception, int>? action)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (retry < retryCount)
                {
                    retry++;
                    return SafeAction(func, action);
                }

                if (action is { } next) next(e, retry);

                throw;
            }
        }
    }
}