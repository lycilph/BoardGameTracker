namespace BoardGameTracker.Application.Common.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

    public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
}
