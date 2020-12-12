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
            return instructions.Aggregate(new PositionAndVector(new Position(0, 0), East),
                (previous, instruction) =>
                {
                    switch (instruction.Action)
                    {
                        case 'N':
                            return new PositionAndVector(previous.Position + North * instruction.Value, previous.Vector);
                        case 'E':
                            return new PositionAndVector(previous.Position + East * instruction.Value, previous.Vector);
                        case 'S':
                            return new PositionAndVector(previous.Position + South * instruction.Value, previous.Vector);
                        case 'W':
                            return new PositionAndVector(previous.Position + West * instruction.Value, previous.Vector);
                        case 'L':
                            return new PositionAndVector(previous.Position, RotateVector(previous.Vector, 360 - instruction.Value));
                        case 'R':
                            return new PositionAndVector(previous.Position, RotateVector(previous.Vector, instruction.Value));
                        case 'F':
                            return new PositionAndVector(previous.Position + previous.Vector * instruction.Value, previous.Vector);
                        default:
                            throw new ApplicationException();
                    }
                })
            .Position.ManhattanDistance();
        }

        private static long Part2(List<Instruction> instructions)
        {
            return instructions.Aggregate(new PositionAndVector(new Position(0, 0), new Vector(10, 1)),
                (previous, instruction) =>
                {
                    switch (instruction.Action)
                    {
                        case 'N':
                            return new PositionAndVector(previous.Position, previous.Vector + North * instruction.Value);
                        case 'E':
                            return new PositionAndVector(previous.Position, previous.Vector + East * instruction.Value);
                        case 'S':
                            return new PositionAndVector(previous.Position, previous.Vector + South * instruction.Value);
                        case 'W':
                            return new PositionAndVector(previous.Position, previous.Vector + West * instruction.Value);
                        case 'L':
                            return new PositionAndVector(previous.Position, RotateVector(previous.Vector, 360 - instruction.Value));
                        case 'R':
                            return new PositionAndVector(previous.Position, RotateVector(previous.Vector, instruction.Value));
                        case 'F':
                            return new PositionAndVector(previous.Position + previous.Vector * instruction.Value, previous.Vector);
                        default:
                            throw new ApplicationException();
                    }
                })
            .Position.ManhattanDistance();
        }


        static readonly Vector North = new Vector(0, 1);
        static readonly Vector East = new Vector(1, 0);
        static readonly Vector South = new Vector(0, -1);
        static readonly Vector West = new Vector(-1, 0);

        public static Vector RotateVector(Vector vector, int degreesRight)
        {
            return degreesRight switch
            {
                0 => vector,
                90 => new Vector(vector.dY, -vector.dX),
                180 => new Vector(-vector.dX, -vector.dY),
                270 => new Vector(-vector.dY, vector.dX),
                _ => throw new ApplicationException()
            };
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

        public class PositionAndVector
        {
            public PositionAndVector(Position position, Vector vector)
            {
                Position = position;
                Vector = vector;
            }

            public Position Position { get; }
            public Vector Vector { get; }
        }
    }
}