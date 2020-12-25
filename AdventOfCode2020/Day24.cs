using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day24
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day24Input.txt"));

            var sample = ConvertInput(File.ReadAllText("Inputs/Day24Sample.txt"));
            var timer = new MyTimer();

            Run(sample).Should().Be(10);
            Run(input).Should().Be(386);

            timer.Lap();

            Run2(sample, 100).Should().Be(2208);
            timer.Lap();
            Run2(input, 100).Should().Be(4214);
            timer.Lap();
            timer.Total();
        }

        private static Vector SouthEast => new Vector(0, 1);
        private static Vector SouthWest => new Vector(-1, 1);
        private static Vector West => new Vector(-1, 0);
        private static Vector NorthWest => new Vector(0, -1);
        private static Vector NorthEast => new Vector(1, -1);
        private static Vector East => new Vector(1, 0);

        private static long Run(List<string> input)
        {
            var tiles = Initialize(input);
            return tiles.Count;
        }

        private static HashSet<Position> Initialize(List<string> input)
        {
            var counts = new DictionaryWithDefault<Position, int>(0);
            foreach (var s in input)
            {
                var directions = Regex.Matches(s, "se|sw|nw|ne|e|w");
                var key = directions.Aggregate(Position.Zero, (position, match) => match.Value switch
                {
                    "se" => position + SouthEast,
                    "sw" => position + SouthWest,
                    "w" => position + West,
                    "nw" => position + NorthWest,
                    "ne" => position + NorthEast,
                    "e" => position + East,
                    _ => throw new ApplicationException()
                });

                counts[key] += 1;
            }

            return counts.Where(kv => IsBlack(kv.Value)).Select(kv => kv.Key).ToHashSet();
        }

        private static long Run2(List<string> input, int iterations)
        {
            var initialState = Initialize(input);
            var finalState = Produce.Iterate(initialState, iterations, state =>
            {
                var nextState = new HashSet<Position>();

                var adjacents = new DictionaryWithDefault<Position, int>(0);

                foreach (var position in state.SelectMany(p => Adjacents(p)))
                {
                    adjacents[position] += 1;
                }

                foreach (var (position, adjacentBlacks) in adjacents)
                {
                    if (state.Contains(position))
                    {
                        if (adjacentBlacks == 0 || adjacentBlacks > 2)
                        {
                            // flips to white, do nothing (cell default is white)
                        }
                        else
                        {
                            nextState.Add(position);
                        }
                    }
                    else
                    {
                        if (adjacentBlacks == 2)
                        {
                            nextState.Add(position);
                        }
                    }
                }

                return nextState;
            });

            return finalState.Count;
        }

        private static IEnumerable<Position> Adjacents(Position p)
        {
            yield return p + NorthEast;
            yield return p + East;
            yield return p + SouthEast;
            yield return p + SouthWest;
            yield return p + West;
            yield return p + NorthWest;
        }

        private static bool IsBlack(int i) => i % 2 == 1;

        private static List<string> ConvertInput(string input)
        {
            return input.SplitIntoLines();
        }
    }
}