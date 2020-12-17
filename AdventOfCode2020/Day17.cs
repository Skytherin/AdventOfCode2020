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

        private static long Run(HashSet<MultiDimensionalPosition> initialState)
        {
            return Produce.Iterate(initialState, 6, current =>
                {
                    var adjacentCounts = new DictionaryWithDefault<MultiDimensionalPosition, int>(0);

                    // Look at all active cells and all cells adjacent to active cells
                    var cells = new HashSet<MultiDimensionalPosition>();
                    foreach (var position in current)
                    {
                        cells.Add(position);
                        foreach (var pos2 in position.Adjacents())
                        {
                            cells.Add(pos2);
                            adjacentCounts[pos2] += 1;
                        }
                    }

                    var next = cells.Where(position =>
                    {
                        var active = current.Contains(position);
                        return (active && adjacentCounts[position].IsInRange(2, 3)) ||
                               (!active && adjacentCounts[position] == 3);
                    }).ToHashSet();

                    return next;
                })
                .Count;
        }

        private static HashSet<MultiDimensionalPosition> ConvertInput(string input, int dimensionality)
        {
            var result = new HashSet<MultiDimensionalPosition>();
            foreach (var (line,row) in input.SplitIntoLines().Select((line, row) => (line, row)))
            {
                foreach (var (c,col) in line.Select((c, col) => (c, col)))
                {
                    if (c == '#') 
                        result.Add(new MultiDimensionalPosition(col, row).SetDimensionality(dimensionality));
                }
            }

            return result;
        }
    }
}