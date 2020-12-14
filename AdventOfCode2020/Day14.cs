using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day14
    {
        private static readonly List<Day14Input> Input = ConvertInput(File.ReadAllText("Inputs/Day14.txt"));
        private static readonly List<Day14Input> Sample = ConvertInput(@"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0");
        private static readonly List<Day14Input> Sample2 = ConvertInput(@"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1");

        public static void Run()
        {
            var timer = new MyTimer();
            Part1(Sample).Should().Be(165);
            Part1(Input).Should().Be(5902420735773L); 

            timer.Lap();

            //Part2(Sample).Should().Be(286);
            //Part2(Input).Should().Be(46530);

            timer.Lap();
            timer.Total();
        }

        private static long Part1(List<Day14Input> instructions)
        {
            var memory = new Dictionary<int, long>();
            var maskAnd = long.MaxValue;
            var maskOr = 0L;
            foreach (var instruction in instructions)
            {
                if (!string.IsNullOrWhiteSpace(instruction.Mask))
                {
                    maskAnd = long.MaxValue;
                    maskOr = 0;
                    foreach (var (x, index) in instruction.Mask.Reverse().Select((x,index)=>(x,index)))
                    {
                        if (x == '1')
                        {
                            maskOr |= (1L << index);
                        }

                        if (x == '0')
                        {
                            maskAnd &= ~(1L << index);
                        }
                    }
                }
                else if (instruction.MemAddress is {} memAddress && instruction.MemValue is {} memValue)
                {
                    memory[memAddress] = memValue & maskAnd | maskOr;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return memory.Sum(it => (long)it.Value);
        }

        private static long Part2(List<Day14Input> instructions)
        {
            return 0;
        }

        private static List<Day14Input> ConvertInput(string input)
        {
            return input.SplitIntoLines()
                .Select(line => RegexUtils.Deserialize<Day14Input>(line, @"(mask = (?<Mask>[X0-9]+))|(mem\[(?<MemAddress>\d+)] = (?<MemValue>\d+))"))
                .ToList();
        }

        public class Day14Input
        {
            public string? Mask { get; set; }
            public int? MemAddress { get; set; }
            public long? MemValue { get; set; }
        }
    }
}