using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using AdventOfCode2020.Utils;

namespace AdventOfCode2020
{
    public class Day3
    {
        private static readonly Day3Input SampleInput = new Day3Input(@"..##.......
#...#...#..
.#....#..#.
..#.#...#.#
.#...##..#.
..#.##.....
.#.#.#....#
.#........#
#.##...#...
#...##....#
.#..#...#.#");

        private static readonly Day3Input Input = new Day3Input(File.ReadAllText("Inputs/Day3.txt"));

        public static void Run()
        {
            Part1().Should().BeEquivalentTo(7, 230);
            Part2().Should().BeEquivalentTo(336, 9533698720);
        }

        public static IEnumerable<long> Part1()
        {
            yield return RunAlgorithm1(SampleInput, 3, 1);
            yield return RunAlgorithm1(Input, 3, 1);
        }

        public static IEnumerable<long> Part2()
        {
            yield return RunAlgorithm2(SampleInput);
            yield return RunAlgorithm2(Input);
        }

        private static long RunAlgorithm2(Day3Input input)
        {
            return RunAlgorithm1(input, 1, 1) *
                   RunAlgorithm1(input, 3, 1) *
                   RunAlgorithm1(input, 5, 1) *
                   RunAlgorithm1(input, 7, 1) *
                   RunAlgorithm1(input, 1, 2);
        }


        private static long RunAlgorithm1(Day3Input input, int right, int down)
        {
            return Positions(input, right, down).Count(pos => pos.IsTree);
        }

        private static IEnumerable<Position> Positions(Day3Input input, int right, int down)
        {
            var position = new Position(input, 0, 0);
            while (position.Valid)
            {
                yield return position;
                position = new Position(input, (position.X + right) % input.Width, position.Y + down);
            }
        }
    }

    public class Position
    {
        private readonly Day3Input Input;
        public bool Valid => Y < Input.Height && X < Input.Width;
        public int X { get; }
        public int Y { get; }
        public bool IsTree => Input.Data[Y][X] == '#';

        public Position(Day3Input input, int x, int y)
        {
            Input = input;
            X = x;
            Y = y;
        }
    }

    public class Day3Input
    {
        public Day3Input(string input)
        {
            Data = input.Split("\n").Select(line => line.Trim()).ToArray();
            Width = Data[0].Length;
        }

        public int Width { get; }
        public int Height => Data.Length;
        public string[] Data { get; }
    }
}