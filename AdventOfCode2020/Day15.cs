using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day15
    {
        public static void Run()
        {
            var timer = new MyTimer();
            Part1(0, 3, 6).Should().Be(436);
            Part1(1, 3, 2).Should().Be(1);
            Part1(2,1,3).Should().Be(10);
            Part1(1, 2, 3).Should().Be(27);
            Part1(2, 3, 1).Should().Be(78);
            Part1(3, 2, 1).Should().Be(438);
            Part1(3,1,2).Should().Be(1836);
            Part1(9, 3, 1, 0, 8, 4).Should().Be(371); 

            timer.Lap();

            Part2(0, 3, 6).Should().Be(175594);
            timer.Lap();
            Part2(9, 3, 1, 0, 8, 4).Should().Be(352);

            //Part2(Sample2).Should().Be(208);
            //Part2(Input).Should().Be(3801988250775L);

            timer.Lap();
            timer.Total();
        }

        private static long Part1(params long[] instructions)
        {
            return Algorithm(2020, instructions);
        }

        private static long Part2(params long[] instructions)
        {
            return Algorithm(30000000, instructions);
        }

        private static long Algorithm(long sentinel, params long[] instructions)
        {
            var turn = 1;
            var numbers = new Dictionary<long, (long penultimateSpoken, long lastSpoken)>();
            
            foreach (var i in instructions)
            {
                numbers[i] = (turn, turn);
                turn++;
            }

            var lastSpoken = instructions.Last();
            var lastSpokenKey = numbers[lastSpoken];

            for ( ; turn <= sentinel; turn++)
            {
                var (penultimateSpoken, lastTurnSpoken) =
                    numbers.ContainsKey(lastSpoken)
                        ? numbers[lastSpoken]
                        : (turn - 1, turn - 1);
                lastSpoken = lastTurnSpoken - penultimateSpoken;

                if (numbers.TryGetValue(lastSpoken, out var temp))
                {
                    numbers[lastSpoken] = (temp.lastSpoken, turn);
                }
                else
                {
                    numbers[lastSpoken] = (turn, turn);
                }
            }

            return lastSpoken;
        }
    }

}