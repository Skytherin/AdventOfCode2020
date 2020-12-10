using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public class Day10
    {
        private static readonly long[] Input = ConvertInput(File.ReadAllText("Inputs/Day10.txt"));

        public static void Run()
        {
            Part1();
            Part2();
        }

        public static void Part1()
        {
            var differences = Input
                .OrderBy(it => it)
                .Prepend(0)
                .Pairs((tail, current) => current - tail)
                .Append(3)
                .GroupBy(it => it)
                .ToDictionary(it => it.Key, it => it.ToList());
            var result = differences[1].Count * differences[3].Count;
            result.Should().Be(2240);
        }

        public static void Part2()
        {
            var sample = ConvertInput(@"16
10
15
5
1
11
7
19
6
12
4");

            RunAlgorithm2(sample).Should().Be(8);
            RunAlgorithm2(Input).Should().Be(99214346656768);
        }

        private static long RunAlgorithm2(long[] input)
        {
            input = input.Prepend(0).OrderBy(it => it).ToArray();
            var waysIn = new long[input.Length];

            waysIn[0] = 1;

            for (var i = 0; i < input.Length; ++i)
            {
                var current = input[i];
                var waysIntoCurrent = waysIn[i];
                for (var j = 1; j <= 3; ++j)
                {
                    var next = i + j;
                    if (next >= input.Length || input[next] > current + 3) break;
                    waysIn[next] = waysIn[next] + waysIntoCurrent;
                }
            }

            return waysIn.Last();
        }


        private static long[] ConvertInput(string input) =>
            input.SplitIntoLines().Select(it => Convert.ToInt64(it)).ToArray();
    }
}