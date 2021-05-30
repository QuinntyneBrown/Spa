using static System.Linq.Enumerable;

namespace Spa.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Indent(this string value, int indent)
            => $"{string.Join("", Range(1, 2 * indent).Select(i => ' '))}{value}";
        
    }
}
