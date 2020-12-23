using System;
using System.Collections.Generic;
using System.Linq;

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

        public static TState Iterate<TState>(TState initialState, int iterations, Func<TState, TState> func)
        {
            for (var i = 0; i < iterations; ++i)
            {
                initialState = func(initialState);
            }

            return initialState;
        }
    }
}
