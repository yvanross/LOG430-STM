namespace Application.Common.Extensions;

public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.Any();
    }
}