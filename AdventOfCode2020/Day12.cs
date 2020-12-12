using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day12
    {
        private static readonly List<Instruction> Input = ConvertInput(File.ReadAllText("Inputs/Day12.txt"));
        private static readonly List<Instruction> Sample = ConvertInput(@"F10
N3
F7
R90
F11");

        public static void Run()
        {
            var timer = new MyTimer();
            Part1(Sample).Should().Be(25);
            Part1(Input).Should().Be(1177);

            timer.Lap();

            Part2(Sample).Should().Be(286);
            Part2(Input).Should().Be(46530);

            timer.Lap();
            timer.Total();
        }

        private static long Part1(List<Instruction> instructions)
        {
            var facing = East;
            var position = new Position(0, 0);

            foreach (var instruction in instructions)
            {
                switch (instruction.Action)
                {
                    case 'N':
                        position = position.Add(North, instruction.Value);
                        break;
                    case 'E':
                        position = position.Add(East, instruction.Value);
                        break;
                    case 'S':
                        position = position.Add(South, instruction.Value);
                        break;
                    case 'W':
                        position = position.Add(West, instruction.Value);
                        break;
                    case 'L':
                        facing = RotateVector(facing, 360 - instruction.Value);
                        break;
                    case 'R':
                        facing = RotateVector(facing, instruction.Value);
                        break;
                    case 'F':
                        position = position.Add(facing, instruction.Value);
                        break;
                    default:
                        throw new ApplicationException();
                }
            }

            return position.ManhattanDistance();
        }

        private static long Part2(List<Instruction> instructions)
        {
            var vector = new Vector(10, 1);
            var position = new Position(0, 0);

            foreach (var instruction in instructions)
            {
                switch (instruction.Action)
                {
                    case 'N':
                        vector = vector.Add(North, instruction.Value);
                        break;
                    case 'E':
                        vector = vector.Add(East, instruction.Value);
                        break;
                    case 'S':
                        vector = vector.Add(South, instruction.Value);
                        break;
                    case 'W':
                        vector = vector.Add(West, instruction.Value);
                        break;
                    case 'L':
                        vector = RotateVector(vector, 360 - instruction.Value);
                        break;
                    case 'R':
                        vector = RotateVector(vector, instruction.Value);
                        break;
                    case 'F':
                        position = position.Add(vector, instruction.Value);
                        break;
                    default:
                        throw new ApplicationException();
                }
            }

            return position.ManhattanDistance();
        }


        static readonly Vector North = new Vector(0, 1);
        static readonly Vector East = new Vector(1, 0);
        static readonly Vector South = new Vector(0, -1);
        static readonly Vector West = new Vector(-1, 0);

        public static Vector RotateVector(Vector vector, int degreesRight)
        {
            var steps = degreesRight / 90;
            while (steps-- > 0)
            {
                vector = new Vector(vector.dY, -vector.dX);
            }

            return vector;
        }

        private static List<Instruction> ConvertInput(string input)
        {
            return RegexUtils.DeserializeMany<Instruction>(input, @"(?<Action>[NESWLRF])(?<Value>\d+)");
        }


        public class Instruction
        {
            public char Action { get; set; }
            public int Value { get; set; }
        }
    }
}