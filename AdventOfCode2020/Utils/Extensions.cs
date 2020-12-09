﻿using System;
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
    }
}