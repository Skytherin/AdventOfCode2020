using System.Linq;
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

        private static long Algorithm(int sentinel, params long[] instructions)
        {
            var turn = 1;
            var numbers = new int[sentinel];
            
            foreach (var i in instructions)
            {
                numbers[i] = turn;
                turn++;
            }

            var lastSpokenNumber = instructions.Last();
            var penultimateSpokenTurn = numbers[lastSpokenNumber];
            for ( ; turn <= sentinel; turn++)
            {
                lastSpokenNumber = turn - penultimateSpokenTurn - 1;

                penultimateSpokenTurn = numbers[lastSpokenNumber];
                if (penultimateSpokenTurn == 0) penultimateSpokenTurn = turn;

                numbers[lastSpokenNumber] = turn;
            }

            return lastSpokenNumber;
        }
    }
}