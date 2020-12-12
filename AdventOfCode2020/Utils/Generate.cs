using System;
using System.Collections.Generic;

namespace AdventOfCode2020.Utils
{
    public static class Generate
    {
        public static IEnumerable<T> Infinite<T>(T initial, Func<T, T> generateNext)
        {
            var current = initial;
            while (true)
            {
                current = generateNext(current);
                yield return current;
            }
        }
    }
}