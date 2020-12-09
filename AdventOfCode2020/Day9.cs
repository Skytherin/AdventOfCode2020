using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public class Day9
    {
        private static readonly long[] Input = ConvertInput(File.ReadAllText("Inputs/Day9.txt"));

        private static readonly long[] Sample = ConvertInput(@"35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576");

        public static void Run()
        {
            Part1();
            Part2();
        }

        public static void Part1()
        {
            FindWeakness(Sample, 5).Should().Be(127);

            FindWeakness(Input, 25).Should().Be(50047984L);
        }

        public static void Part2()
        {
            ExploitWeakness(Sample, 5).Should().Be(62);

            ExploitWeakness(Input, 25).Should().Be(5407707L);
        }

        private static long FindWeakness(long[] input, int preambleLength)
        {
            return input[FindWeaknessIndex(input, preambleLength)];
        }

        private static int FindWeaknessIndex(long[] input, int preambleLength)
        {
            for (var current = preambleLength; current < input.Length; ++current)
            {
                var sums = new HashSet<long>();
                var found = false;
                for (var tail = current - preambleLength; tail < current; ++tail)
                {
                    if (sums.Contains(input[tail]))
                    {
                        found = true;
                        break;
                    }
                    sums.Add(input[current] - input[tail]);
                }

                if (!found) return current;
            }

            throw new ApplicationException();
        }

        private static long ExploitWeakness(long[] input, int preambleLength)
        {
            var weaknessIndex = FindWeaknessIndex(input, preambleLength);
            var weakness = input[weaknessIndex];
            var current = new SummedList();
            for (var i = 0; i < weaknessIndex; ++i)
            {
                current.Add(input[i]);

                while (current.Sum > weakness)
                {
                    current.Shift();
                }

                if (current.Count >= 2 && current.Sum == weakness)
                {
                    return current.List.Max() + current.List.Min();
                }
            }
            throw new ApplicationException();
        }

        private static long[] ConvertInput(string input) =>
            input.SplitIntoLines().Select(it => Convert.ToInt64(it)).ToArray();
    }
}