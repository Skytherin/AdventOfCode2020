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
        private static readonly ConwayCell[][] Input = ConvertInput(File.ReadAllText("Inputs/Day11.txt"));
        private static readonly ConwayCell[][] Sample = ConvertInput(@"L.LL.LL.LL
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
            Console.WriteLine($"{(DateTime.Now - dtin).TotalMilliseconds}");
            Part2(Sample.ConwayClone()).Should().Be(26);
            Part2(Input.ConwayClone()).Should().Be(1944);
            Console.WriteLine($"{(DateTime.Now - dtin).TotalMilliseconds}");
        }

        public static long Part1(ConwayCell[][] conwayCells) => ConwayLives(conwayCells, 4, KingsNeighbors);

        public static long Part2(ConwayCell[][] conwayCells) => ConwayLives(conwayCells, 5, QueensNeighbors);

        public static long ConwayLives(ConwayCell[][] grid, int deathTolerance, 
            Func<ConwayCell[][], Position, IEnumerable<Position>> neighborGenerator)
        {
            var seatsPositions = grid
                .SelectMany((row, y) => row
                    .Select((col, x) => new {x,y,col})
                    .Where(value => value.col.Value.IsSeat())
                    .Select(it => new Position(it.x, it.y)))
                .ToList();

            foreach (var position in seatsPositions)
            {
                grid.At(position).Neighbors = neighborGenerator(grid, position).Select(nb => grid.At(nb)).ToList();
            }

            var seats = seatsPositions.Select(pos => grid.At(pos)).ToList();

            while (Next(deathTolerance, seats))
            {
                // Do nothing
            }

            return seats.Count(seat => seat.Value.IsOccupiedSeat());
        }

        private static bool Next(int deathTolerance, List<ConwayCell> seats)
        {
            seats.ForEach(seat => seat.NeighborCount = 0);

            foreach (var seat in seats.Where(seat => seat.Value.IsOccupiedSeat()))
            {
                foreach (var neighbor in seat.Neighbors)
                {
                    neighbor.NeighborCount += 1;
                }
            }

            var changed = false;
            foreach (var seat in seats)
            {
                if (seat.Value.IsOccupiedSeat())
                {
                    if (seat.NeighborCount >= deathTolerance)
                    {
                        changed = true;
                        seat.Value = Conway.Empty;
                    }
                }
                else if (seat.NeighborCount == 0)
                {
                    changed = true;
                    seat.Value = Conway.Occupied;
                }
            }

            return changed;
        }

        private static void Visualize(char[][] current)
        {
            foreach (var row in current)
            {
                Console.WriteLine(row.Select(it => $"{it}").Join(""));
            }

            Console.WriteLine();
        }

        private static IEnumerable<Position> KingsNeighbors(ConwayCell[][] grid, Position position)
        {
            return Directions().SelectMany(direction => Walk(grid, position, direction)
                .Take(1)
                .Where(c => grid.At(c).Value.IsSeat()));
        }

        private static IEnumerable<Position> QueensNeighbors(ConwayCell[][] grid, Position position)
        {
            return Directions().SelectMany(direction => Walk(grid, position, direction)
                .Where(c => grid.At(c).Value.IsSeat())
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

        private static IEnumerable<Position> Walk<T>(T[][] grid, Position position, Vector direction)
        {
            var stepRow = position.Y + direction.Y;
            var stepCol = position.X + direction.X;
            while (stepCol >= 0 && stepCol < grid[0].Length && 
                   stepRow >= 0 && stepRow < grid.Length)
            {
                yield return new Position(stepCol, stepRow);
                stepRow += direction.Y;
                stepCol += direction.X;
            }
        }

        private static ConwayCell[][] ConvertInput(string input)
        {
            var result = input.SplitIntoLines()
                .Select(it => it.Select(c => new ConwayCell
                {
                    Value = c
                }).ToArray())
                .ToArray();

            // Sanity check all rows have same length
            result.Select(col => col.Length).Should().AllBeEquivalentTo(result[0].Length);

            return result;
        }

    }

    public static class Conway
    {
        public const char Occupied = '#';
        public const char Empty = 'L';

        public static ConwayCell[][] ConwayClone(this ConwayCell[][] current)
        {
            return current
                .Select(it => it.Select(x => new ConwayCell
                {
                    Value = x.Value
                }).ToArray())
                .ToArray();
        }

        public static T2[][] ConwayClone<T, T2>(this T[][] current, Func<T, T2> convert)
        {
            return current
                .Select(it => it.Select(x => convert(x)).ToArray())
                .ToArray();
        }

        public static bool IsSeat(this char c) => c == Occupied || c == Empty;
        public static bool IsOccupiedSeat(this char c) => c == Occupied;
        public static bool IsEmptySeat(this char c) => c == Empty;
    }

    public class ConwayCell
    {
        public char Value { get; set; }
        public List<ConwayCell> Neighbors = new List<ConwayCell>();
        public int NeighborCount = 0;
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