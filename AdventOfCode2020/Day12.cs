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
            return instructions.Aggregate(new {Position = new Position(0, 0), Vector = Vector.East},
                (previous, instruction) =>
                {
                    return instruction.Action switch
                    {
                        'N' => new {Position = previous.Position + Vector.North * instruction.Value, previous.Vector},
                        'E' => new {Position = previous.Position + Vector.East * instruction.Value, previous.Vector},
                        'S' => new {Position = previous.Position + Vector.South * instruction.Value, previous.Vector},
                        'W' => new {Position = previous.Position + Vector.West * instruction.Value, previous.Vector},
                        'L' => new {previous.Position, Vector = RotateVector(previous.Vector, 360 - instruction.Value)},
                        'R' => new {previous.Position, Vector = RotateVector(previous.Vector, instruction.Value)},
                        'F' => new {Position = previous.Position + previous.Vector * instruction.Value, previous.Vector},
                        _ => throw new ApplicationException()
                    };
                })
            .Position.ManhattanDistance();
        }

        private static long Part2(List<Instruction> instructions)
        {
            return instructions.Aggregate(new { Position = new Position(0, 0), Vector = new Vector(10, 1) },
                    (previous, instruction) =>
                    {
                        return instruction.Action switch
                        {
                            'N' => new { previous.Position, Vector = previous.Vector + Vector.North * instruction.Value },
                            'E' => new { previous.Position, Vector = previous.Vector + Vector.East * instruction.Value },
                            'S' => new { previous.Position, Vector = previous.Vector + Vector.South * instruction.Value },
                            'W' => new { previous.Position, Vector = previous.Vector + Vector.West * instruction.Value },
                            'L' => new { previous.Position, Vector = RotateVector(previous.Vector, 360 - instruction.Value) },
                            'R' => new { previous.Position, Vector = RotateVector(previous.Vector, instruction.Value) },
                            'F' => new { Position = previous.Position + previous.Vector * instruction.Value, previous.Vector },
                            _ => throw new ApplicationException()
                        };
                    })
                .Position.ManhattanDistance();
        }


        

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
    }
}