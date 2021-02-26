// unset

namespace Authentication.API.Extensions
{
    public static class StringExtensions
    {
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}