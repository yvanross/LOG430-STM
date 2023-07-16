namespace Entities.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string a, string? b)
    {
        if (b is null) return false;

        return ReferenceEquals(a, b) || string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}