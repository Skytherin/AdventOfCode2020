using System;
using System.Diagnostics;

namespace AdventOfCode2020.Utils
{
    public class MyTimer
    {
        private readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        private TimeSpan LastLap = TimeSpan.Zero;
        private int LapCount = 0;

        public void Lap()
        {
            var elapsed = Stopwatch.Elapsed;
            Console.WriteLine($"Lap {LapCount++}: {(elapsed - LastLap).TotalSeconds}");
            LastLap = elapsed;
        }

        public void Total()
        {
            Console.WriteLine($"Total: {Stopwatch.Elapsed.TotalSeconds}");
        }
    }
}