namespace Shared.Abstraction
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
        public static bool IsNullOrWhitespace(this string value) => string.IsNullOrWhiteSpace(value);
    }
}