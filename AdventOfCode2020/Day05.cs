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
    public class Day05
    {
        private static readonly HashSet<long> Input = File.ReadAllText("Inputs/Day05.txt").Split("\n")
            .Select(it => RunAlgorithm1(it.Trim())).ToHashSet();

        public static void Run()
        {
            RunAlgorithm1("FBFBBFFRLR").Should().Be(357);
            RunAlgorithm1("BFFFBBFRRR").Should().Be(567);
            RunAlgorithm1("FFFBBBFRRR").Should().Be(119);
            RunAlgorithm1("BBFFBBFRLL").Should().Be(820);
            Part1().Should().Be(987);
            Part2().Should().Be(603);
        }

        public static long Part1()
        {
            return Input.Max();
        }

        public static long Part2()
        {
            var max = Input.Max();
            var min = Input.Min();

            for (var potential = min + 1; potential < max - 1; potential++)
            {
                if (!Input.Contains(potential) &&
                    Input.Contains(potential - 1) &&
                    Input.Contains(potential + 1))
                {
                    return potential;
                }
            }

            throw new ApplicationException();
        }

        private static long RunAlgorithm1(string input)
        {
            return input.Aggregate(0L, (accum, value) => accum * 2 + (value == 'B' || value == 'R' ? 1 : 0));
        }
    }
}