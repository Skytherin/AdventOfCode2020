using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day11
    {
        private static readonly char[][] Input = ConvertInput(File.ReadAllText("Inputs/Day11.txt"));
        private static readonly char[][] Sample = ConvertInput(@"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL");

        public static void Run()
        {
            var dtin = DateTime.Now;
            Part1(Sample.ConwayClone()).Should().Be(37);
            Part1(Input.ConwayClone()).Should().Be(2194);
            
            Part2(Sample.ConwayClone()).Should().Be(26);
            Part2(Input.ConwayClone()).Should().Be(1944);
            Console.WriteLine($"{(DateTime.Now - dtin).TotalMilliseconds}");
        }

        public static long Part1(char[][] conwayCells) => ConwayLives(conwayCells, 4, KingsNeighbors);

        public static long Part2(char[][] conwayCells) => ConwayLives(conwayCells, 5, QueensNeighbors);

        public static long ConwayLives(char[][] conwayCells, int deathTolerance, Func<char[][], int, int, IEnumerable<char>> neighbors)
        {
            var current = conwayCells;
            var next = conwayCells.ConwayZero();
            while (!current.HasStabalized(next))
            {
                // Visualize(current);
                for (var row = 0; row < conwayCells.Length; ++row)
                {
                    for (var col = 0; col < conwayCells[0].Length; ++col)
                    {
                        var neighborCount = neighbors(current, row, col).Count(n => n == '#');
                        if (current[row][col] == '#' && neighborCount >= deathTolerance)
                        {
                            next[row][col] = 'L';
                        }
                        else if (current[row][col] == 'L' && neighborCount == 0)
                        {
                            next[row][col] = '#';
                        }
                        else
                        {
                            next[row][col] = current[row][col];
                        }
                    }
                }

                (current, next) = (next, current);
            }

            return conwayCells.Sum(it => it.Count(c => c == '#'));
        }

        private static void Visualize(char[][] current)
        {
            foreach (var row in current)
            {
                Console.WriteLine(row.Select(it => $"{it}").Join(""));
            }

            Console.WriteLine();
        }

        private static IEnumerable<char> KingsNeighbors(char[][] conwayCells, int row, int col)
        {
            return Directions().SelectMany(direction => Walk(conwayCells, row, col, direction).Take(1));
        }

        private static IEnumerable<char> QueensNeighbors(char[][] conwayCells, int row, int col)
        {
            return Directions().SelectMany(direction => Walk(conwayCells, row, col, direction)
                .Where(c => c == '#' || c == 'L')
                .Take(1));
        }

        private static IEnumerable<Vector> Directions()
        {
            foreach (var y in new[] {-1, 0, 1})
            {
                foreach (var x in new[] {-1, 0, 1}.Where(c => y != 0 || c != 0))
                {
                    yield return new Vector(x, y);
                }
            }
        }

        private static IEnumerable<char> Walk(char[][] conwayCells, int row, int col, Vector direction)
        {
            var stepRow = row + direction.Y;
            var stepCol = col + direction.X;
            while (stepCol >= 0 && stepCol < conwayCells[0].Length && 
                   stepRow >= 0 && stepRow < conwayCells.Length)
            {
                yield return conwayCells[stepRow][stepCol];
                stepRow += direction.Y;
                stepCol += direction.X;
            }
        }

        private static char[][] ConvertInput(string input)
        {
            var result = input.SplitIntoLines()
                .Select(it => it.Select(c => c).ToArray())
                .ToArray();

            // Sanity check all rows have same length
            result.Select(col => col.Length).Should().AllBeEquivalentTo(result[0].Length);

            return result;
        }

    }

    public static class ConwayExtensions
    {
        public static bool HasStabalized(this char[][] current, char[][] previous)
        {
            return current
                .SelectMany(it => it)
                .Zip(previous.SelectMany(it => it), (c, c1) => new {c, c1})
                .All(c => c.c == c.c1);
        }

        public static char[][] ConwayClone(this char[][] current)
        {
            return current
                .Select(it => it.ToArray())
                .ToArray();
        }

        public static char[][] ConwayZero(this char[][] current)
        {
            return current
                .Select(it => it.Select(_ => ' ').ToArray())
                .ToArray();
        }
    }

    public class Vector
    {
        public int X { get; }
        public int Y { get; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}