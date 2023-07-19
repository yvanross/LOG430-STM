namespace Entities.Extensions;

public static class Try
{
    public static async Task<T?> WithConsequenceAsync<T>(Func<Task<T>> todo, Func<Exception, int, Task>? onFailure = null, int retryCount = 0, bool autoThrow = true)
    {
        var retry = 0;

        return await SafeAction(todo, onFailure);

        async Task<T?> SafeAction(Func<Task<T>> func, Func<Exception, int, Task>? onFailure = null)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                if (retry < retryCount)
                {
                    retry++;

                    if(onFailure is not null)
                        await onFailure(e, retry);

                    return await SafeAction(func, onFailure);
                }

                if(autoThrow)
                    throw;

                return await Task.FromResult(default(T));
            }
        }
    }
}