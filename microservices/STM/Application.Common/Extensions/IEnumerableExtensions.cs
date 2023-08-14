namespace Application.Common.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();
}