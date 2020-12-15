using System;
using System.Diagnostics;

namespace AdventOfCode2020.Utils
{
    public class MyTimer
    {
        private readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        private TimeSpan LastLap = TimeSpan.Zero;

        public void Lap()
        {
            var elapsed = Stopwatch.Elapsed;
            Console.WriteLine($"{(elapsed - LastLap).TotalSeconds}");
            LastLap = elapsed;
        }

        public void Total()
        {
            Console.WriteLine($"{Stopwatch.Elapsed.TotalSeconds}");
        }
    }
}