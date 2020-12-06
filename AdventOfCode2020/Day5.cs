using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using FluentAssertions;
using AdventOfCode2020.Utils;

namespace AdventOfCode2020
{
    public class Day6
    {
        private static readonly List<List<string>> Input = File.ReadAllText("Inputs/Day6.txt")
            .RxSplit(@"\s*\n\s*\n\s*")
            .Select(group => group.Split("\n").Select(it=>it.Trim()).ToList())
            .ToList();
            

        public static void Run()
        {
            Part1();
            Part2();
        }

        public static void Part1()
        {
            Input.Sum(group => group
                .Select(line => line.ToHashSet())
                .Aggregate((accum, item) =>
                {
                    accum.UnionWith(item);
                    return accum;
                }).Count
            ).Should().Be(6335);
        }

        public static void Part2()
        {
            Input.Sum(group => group
                .Select(line => line.ToHashSet())
                .Aggregate((accum, item) =>
                {
                    accum.IntersectWith(item);
                    return accum;
                }).Count
            ).Should().Be(3392);
        }
    }
}