using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day17
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day17.txt"), 3);
            var sample = ConvertInput(@".#.
..#
###", 3);

            var timer = new MyTimer();
            Run(sample).Should().Be(112);
            Run(input).Should().Be(319); 

            timer.Lap();

            var input2 = ConvertInput(File.ReadAllText("Inputs/Day17.txt"), 4);
            var sample2 = ConvertInput(@".#.
..#
###", 4);

            Run(sample2).Should().Be(848);
            timer.Lap();
            Run(input2).Should().Be(2324);


            timer.Lap();
            timer.Total();
        }

        private static long Run(DictionaryWithDefault<MultiDimensionalPosition, CubeState> initialState)
        {
            // Visualize(initialState);
            return Produce.Iterate(initialState, 6, current =>
                {
                    var next = new DictionaryWithDefault<MultiDimensionalPosition, CubeState>(CubeState.Inactive);
                    var adjacentCounts = new DictionaryWithDefault<MultiDimensionalPosition, int>(0);

                    // Look at all active cells and all cells adjacent to active cells
                    var cells = new HashSet<MultiDimensionalPosition>();
                    foreach (var (position, _) in current.Where(kv => kv.Value == CubeState.Active))
                    {
                        cells.Add(position);
                        foreach (var pos2 in position.Adjacents())
                        {
                            cells.Add(pos2);
                            adjacentCounts[pos2] += 1;
                        }
                    }

                    foreach (var position in cells)
                    {
                        var value = current[position];
                        if (value == CubeState.Active && adjacentCounts[position].IsInRange(2, 3))
                        {
                            next[position] = CubeState.Active;
                        }
                        else if (value == CubeState.Inactive && adjacentCounts[position] == 3)
                        {
                            next[position] = CubeState.Active;
                        }
                    }

                    // Visualize(next);
                    return next;
                })
                .Count(kv => kv.Value == CubeState.Active);
        }

        private static void Visualize(DictionaryWithDefault<MultiDimensionalPosition, CubeState> next)
        {
            if (!next.Any()) return;
            Console.WriteLine();
            Console.WriteLine("===================");
            var cells = next.ToList();
            for (var z = cells.Min(it => it.Key.Positions[2]);
                z <= cells.Max(it => it.Key.Positions[2]);
                z++)
            {
                Console.WriteLine();
                for (var y = cells.Min(it => it.Key.Positions[1]);
                    y <= cells.Max(it => it.Key.Positions[1]);
                    y++)
                {
                    var s = "";
                    for (var x = cells.Min(it => it.Key.Positions[0]);
                        x <= cells.Max(it => it.Key.Positions[0]);
                        x++)
                    {
                        s += next[new MultiDimensionalPosition(x, y, z)] switch
                        {
                            CubeState.Active => "#",
                            _ => "."
                        };
                    }
                    Console.WriteLine(s);
                }
            }
        }

        private static DictionaryWithDefault<MultiDimensionalPosition, CubeState> ConvertInput(string input, int dimensionality)
        {
            var result = new DictionaryWithDefault<MultiDimensionalPosition, CubeState>(CubeState.Inactive);
            foreach (var (line,row) in input.SplitIntoLines().Select((line, row) => (line, row)))
            {
                foreach (var (c,col) in line.Select((c, col) => (c, col)))
                {
                    result[new MultiDimensionalPosition(col, row).SetDimensionality(dimensionality)] = 
                        c switch
                        {
                            '#' => CubeState.Active,
                            '.' => CubeState.Inactive,
                            _ => throw new ApplicationException()
                        };
                }
            }

            return result;
        }

        public enum CubeState
        {
            Active,
            Inactive
        }
    }
}