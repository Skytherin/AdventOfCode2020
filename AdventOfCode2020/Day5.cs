using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using AdventOfCode2020.Utils;

namespace AdventOfCode2020
{
    public class Day6
    {
        private static readonly List<List<string>> Input = File.ReadAllText("Inputs/Day6.txt")
            .SplitOnBlankLines()
            .Select(group => group.SplitIntoLines())
            .ToList();
            

        public static void Run()
        {
            Part1();
            Part2();
        }

        public static void Part1()
        {
            MergeAndSum((accum, item) => accum.UnionWith(item))
                .Should().Be(6335);
        }

        public static void Part2()
        {
            MergeAndSum((accum, item) => accum.IntersectWith(item))
                .Should().Be(3392);
        }

        public static long MergeAndSum(Action<HashSet<char>, HashSet<char>> action)
        {
            return Input.Sum(group => group
                .Select(line => line.ToHashSet())
                .Aggregate((accum, item) =>
                {
                    action(accum, item);
                    return accum;
                }).Count
            );
        }
    }
}