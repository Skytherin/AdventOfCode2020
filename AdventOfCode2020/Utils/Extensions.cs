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

        public static void EnqueueAll<T>(this Queue<T> self, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                self.Enqueue(item);
            }
        }
    }
}