using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

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

        public static void Append<TKey, TValue>(this Dictionary<TKey, List<TValue>> d, TKey key, TValue value)
            where TKey: notnull
        {
            if (d.ContainsKey(key))
            {
                d[key].Add(value);
            }
            else
            {
                d[key] = new List<TValue> {value};
            }
        }

        public static T Shift<T>(this List<T> self)
        {
            if (self.Any())
            {
                var result = self.First();
                self.RemoveAt(0);
                return result;
            }
            throw new ApplicationException("Attempt to shift empty list.");
        }

        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> input)
        {
            using var enumerator = input.GetEnumerator();
            if (!enumerator.MoveNext()) yield break;
            var tail = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return Tuple.Create(tail, current);
                tail = current;
            }
        }

        public static IEnumerable<TOut> Pairs<T, TOut>(this IEnumerable<T> input, Func<T, T, TOut> map)
        {
            return input.Pairs().Select(pair => map(pair.Item1, pair.Item2));
        }

        public static T At<T>(this T[][] grid, Position p)
        {
            return grid[p.Y][p.X];
        }

        public static T2[][] GridClone<T, T2>(this T[][] current, Func<T, T2> convert)
        {
            return current
                .Select(it => it.Select(x => convert(x)).ToArray())
                .ToArray();
        }

        public static bool IsMultipleOf(this long product, long value) => (product % value) == 0;

        public static long Mod(this long k, long n)
        {
            var result = k % n;
            if (result < 0) return n + result;
            return result;
        }
    }
}