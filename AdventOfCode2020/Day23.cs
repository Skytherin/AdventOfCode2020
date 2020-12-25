using System;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day23
    {
        public static void Run()
        {
            var input = ConvertInput("253149867");

            var sample = ConvertInput("389125467");
            var timer = new MyTimer();

            Run1(sample.Copy(), 10).Should().Be("92658374");
            Run1(sample.Copy(), 100).Should().Be("67384529");
            Run1(input.Copy(), 100).Should().Be("34952786");

            timer.Lap();
            Run2(sample.Copy().AddRange(Enumerable.Range(10, 1_000_000 - 9)), 10_000_000).Should().Be(149245887792);
            timer.Lap();
            Run2(input.Copy().AddRange(Enumerable.Range(10, 1_000_000-9)), 10_000_000).Should().Be(505334281774);
            timer.Lap();


            timer.Total();
        }

        private static string Run1(CircularList<int> initialState, int iterations)
        {
            var one = Run(initialState, iterations);
            return one.Skip(1).Select(it => it.ToString()).Join("");
        }

        private static long Run2(CircularList<int> initialState, int iterations)
        {
            var one = Run(initialState, iterations);
            var n1 = one.Next.Value;
            var n2 = one.Next.Next.Value;
            return (long)n1 * (long)n2;
        }

        private static CircularList<int> Run(CircularList<int> initialState, int iterations)
        {
            var nodeMap = initialState.Walk().ToDictionary(it => it.Value, it => it);
            var numberOfCups = nodeMap.Count;
            Produce.Iterate(initialState, iterations, state =>
            {
                var pickedUp = state.Next.Extract(3);
                var destination = Modthing(state.Value - 1, numberOfCups);
                while (pickedUp.Value == destination || pickedUp.Next.Value == destination || pickedUp.Next.Next.Value == destination)
                {
                    destination = Modthing(destination - 1, numberOfCups);
                }

                var d = nodeMap[destination];
                d.Insert(pickedUp);

                return state.Next;
            });

            return nodeMap[1];
        }

        private static int Modthing(int n, int numberOfCups)
        {
            if (n == 0) return numberOfCups;
            return n;
        }

        private static CircularList<int> ConvertInput(string input)
        {
            return CircularList<int>.From(input.Select(i => Convert.ToInt32(i.ToString())));
        }
    }
}