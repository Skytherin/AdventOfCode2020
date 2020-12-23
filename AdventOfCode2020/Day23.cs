using System;
using System.Collections.Generic;
using System.IO;
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

            Run1(sample.Copy(), 10, 9).Should().Be("92658374");
            Run1(sample.Copy(), 100, 9).Should().Be("67384529");
            Run1(input.Copy(), 100, 9).Should().Be("34952786");


            timer.Lap();

            Run1(input.Copy(), 1000000, 9);
            timer.Lap();


            timer.Total();
        }

        private static string Run1(CircularList<int> initialState, int iterations, int numberOfCups)
        {
            var one = Run(initialState, iterations, numberOfCups);
            return one.Enumerate().Skip(1).Select(it => it.ToString()).Join("");
        }

        private static CircularList<int> Run(CircularList<int> initialState, int iterations, int numberOfCups)
        {
            var final = Produce.Iterate(initialState, iterations, state =>
            {
                var pickedUp = state.Next.Extract(3);
                var destination = Modthing(state.Value - 1, numberOfCups);
                while (pickedUp.Find(it => it == destination) != null)
                {
                    destination = Modthing(destination - 1, numberOfCups);
                }

                var d = state.Find(it => it == destination) ?? throw new ApplicationException();
                d.Insert(pickedUp);

                return state.Next;
            });

            return final.Find(it => it == 1) ?? throw new ApplicationException();
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