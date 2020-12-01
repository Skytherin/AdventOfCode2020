using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;

namespace AdventOfCode2020
{
    public class Day1
    {
        public static void Run()
        {
            Console.WriteLine("=== DAY 1 ===");
            Part1().Should().BeEquivalentTo(514579, 988771);
            Part2().Should().BeEquivalentTo(241861950, 171933104);
        }

        public static IEnumerable<int> Part1()
        {
            Console.WriteLine("=== PART 1 ===");
            var inputs = File.ReadAllLines("Inputs/Day1.txt").Select(it => Convert.ToInt32(it));
            yield return RunAlgorithm1(new []{1721,
                979,
                366,
                299,
                675,
                1456});
            yield return RunAlgorithm1(inputs);
        }

        public static IEnumerable<long> Part2()
        {
            Console.WriteLine("=== PART 2 ===");
            var inputs = File.ReadAllLines("Inputs/Day1.txt").Select(it => Convert.ToInt64(it));
            yield return RunAlgorithm2(new long[]{1721,
                979,
                366,
                299,
                675,
                1456});
            yield return RunAlgorithm2(inputs);
        }

        private static long RunAlgorithm2(IEnumerable<long> inputs)
        {
            var seen = new HashSet<long>();
            var partials = new Dictionary<long, long>();

            foreach (var item in inputs)
            {
                if (partials.TryGetValue(item, out var partial))
                {
                    Console.WriteLine(partial * item);
                    return partial * item;
                }

                foreach (var item2 in seen)
                {
                    partials[2020 - item2 - item] = item * item2;
                }

                seen.Add(item);
            }
            throw new ApplicationException();
        }

        private static int RunAlgorithm1(IEnumerable<int> inputs)
        {
            var seen = new HashSet<int>();
            foreach (var item in inputs)
            {
                var reciprical = 2020 - item;
                if (seen.Contains(reciprical))
                {
                    Console.WriteLine(item * reciprical);
                    return item * reciprical;
                }

                seen.Add(item);
            }
            throw new ApplicationException();
        }
    }
}