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

        public static string Join<T>(this IEnumerable<T> self, string separator) =>
            string.Join(separator, self.Select(it => it?.ToString()));

        public static void EnqueueAll<T>(this Queue<T> self, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                self.Enqueue(item);
            }
        }

        public static void Add<TKey, TValue>(this Dictionary<TKey, List<TValue>> d, TKey key, TValue value)
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

        public static void AddMany<TKey, TValue>(this Dictionary<TKey, List<TValue>> d, TKey key, IEnumerable<TValue> values)
            where TKey : notnull
        {
            if (!d.ContainsKey(key))
            {
                d[key] = new List<TValue>();
            }
            d[key].AddRange(values);
        }

        public static void Add<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> d, TKey key, TValue value)
            where TKey : notnull
        {
            if (d.ContainsKey(key))
            {
                d[key].Add(value);
            }
            else
            {
                d[key] = new HashSet<TValue> { value };
            }
        }

        public static void AddMany<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> d, TKey key, IEnumerable<TValue> values)
            where TKey : notnull
        {
            if (!d.ContainsKey(key))
            {
                d[key] = new HashSet<TValue>();
            }

            d[key].UnionWith(values);
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

        public static T Pop<T>(this List<T> self)
        {
            if (self.Any())
            {
                var result = self.Last();
                self.RemoveAt(self.Count-1);
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

        public static string Right(this string s, int count)
        {
            if (s.Length < count) return s;
            return s.Substring(s.Length - count, count);
        }

        public static (T first, IEnumerable<T> rest) FirstAndRest<T>(this IEnumerable<T> self)
        {
            using var enumerator = self.GetEnumerator();
            if (!enumerator.MoveNext()) throw new ApplicationException();
            var first = enumerator.Current;
            return (first, rest: enumerator.Enumerate());
        }

        public static IEnumerable<T> Enumerate<T>(this IEnumerator<T> self)
        {
            while (self.MoveNext()) yield return self.Current;
        }

        public static int HashWith(this int self, int other)
        {
            return ((self << 16) + (self >> 16)) + other;
        }
    }
}