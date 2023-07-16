namespace Entities.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> WhereNotTrueInAnyOf<T, K>(this IEnumerable<T> source, IEnumerable<K> other, Func<T, K, bool> keySelector)
    {
        return source.Where(t => !other.Any(k => keySelector(t, k)));
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source) where T : class
    {
        return source.Where(t => t is not null)!;
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source) where T : struct
    {
        return source.Where(t => t.HasValue).Select(t=>t!.Value);
    }
}