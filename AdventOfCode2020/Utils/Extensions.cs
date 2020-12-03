using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Utils
{
    public static class Extensions
    {
        public static bool IsInRange(this int self, int min, int max)
        {
            return min <= self && self <= max;
        }

        public static string Join(this IEnumerable<string> self, string separator) =>
            string.Join(separator, self);
    }
}