namespace ServiceMeshHelper.Extensions;

internal static class Try
{
    internal static async Task<T> WithConsequenceAsync<T>(Func<Task<T>> todo, Func<Exception, int, Task>? onFailure = null, int retryCount = 0, bool autoThrow = true, bool quiet = false)
    {
        var retry = 0;

        return await SafeAction(todo, onFailure);

        async Task<T> SafeAction(Func<Task<T>> func, Func<Exception, int, Task>? onFailure = null)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                if (quiet is false) Console.WriteLine(e);

                if (retry < retryCount)
                {
                    retry++;

                    if (onFailure is not null)
                        await onFailure(e, retry);

                    return await SafeAction(func, onFailure);
                }

                if(autoThrow) throw;
                
                return default;
            }
        }
    }
}