using System;
using System.Collections.Generic;

namespace AdventOfCode2020.Utils
{
    public static class Produce
    {
        public static IEnumerable<T> Forever<T>(T initial, Func<T, T> producer)
        {
            while (true)
            {
                yield return initial;
                initial = producer(initial);
            }
        }
    }
}
