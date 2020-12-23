using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day20
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day20.txt"));
            var sample = ConvertInput(File.ReadAllText("Inputs/Day20Sample.txt"));
            var timer = new MyTimer();

            var sample2 = new List<Tile>
            {
                new Tile(new[] {11, 1, 2, 10}, 100, 0),
                new Tile(new[] {14, 15, 4, 1}, 200, 1),
                new Tile(new[] {2, 3, 13, 12}, 300, 2),
                new Tile(new[] {4, 16, 17, 3}, 400, 3)
            };

            var sample3 = new List<Tile>
            {
                new Tile(new[] {11, 1, 2, 10}, 100, 0),
                new Tile(new[] {14, 15, 4, 1}, 200, 1),
                new Tile(new[] {2, 768, 13, 12}, 300, 2),
                new Tile(new[] {17, 16, 4, 3}, 400, 3)
            };

            Run(sample2, 2, out _).Should().Be(100L * 200L * 300L * 400L);
            Run(sample3, 2, out _).Should().Be(100L * 200L * 300L * 400L);
            
            Run(sample, 3, out var sampleGrid).Should().Be(20899048083289);
            Run(input, 12, out var inputGrid).Should().Be(15670959891893L);

            timer.Lap();

            var seaMonster = new List<string>
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            Run2(sampleGrid, seaMonster).Should().Be(273);
            Run2(inputGrid, seaMonster).Should().Be(1964); 

            timer.Total();
        }

        private static long Run2(InfiniteGrid<TileWithOrientation> grid, List<string> seaMonster)
        {
            var bitmap = new List<List<char>>();
            for (var y = grid.Top; y <= grid.Bottom; y++)
            {
                var row = RemoveEdges(grid[new Position(grid.Left, y)].Bitmap);
                for (var x = grid.Left + 1; x <= grid.Right; x++)
                {
                    var tile = RemoveEdges(grid[new Position(x, y)].Bitmap);
                    row = AddInColumn(row, tile);
                }

                if (bitmap.Count == 0) bitmap = row;
                else bitmap = AddInRows(bitmap, row);
            }

            for (var orientation = 0; orientation < 8; orientation++)
            {
                var count = 0;
                var newBitmap = TranslateBitmap(bitmap, orientation).Select(line => line.Join("")).ToList();
                for (var y = 0; y < newBitmap.Count - seaMonster.Count + 1; y++)
                {
                    var index1 = SearchLine(newBitmap[y], seaMonster[0]);
                    var index2 = SearchLine(newBitmap[y+1], seaMonster[1]);
                    var index3 = SearchLine(newBitmap[y+2], seaMonster[2]);
                    var isect = index1.Intersect(index2).Intersect(index3).ToHashSet();
                    if (isect.Any())
                    {
                        count += isect.Count();
                    }
                }

                if (count > 0)
                {
                    return newBitmap.Select(it => it.Count(c => c == '#')).Sum()
                           - seaMonster.Select(it => it.Count(c => c == '#')).Sum() * count;
                }
            }
            throw new ApplicationException();
        }

        private static HashSet<int> SearchLine(string s, string s1)
        {
            var result = new HashSet<int>();
            for (var position = 0; position < s.Length - s1.Length + 1; position++)
            {
                var matched = true;
                for (var index = 0; matched && index < s1.Length; index++)
                {
                    if (s1[index] == ' ') continue;
                    if (s1[index] == '#' && s[position + index] == '#') continue;
                    matched = false;
                }

                if (matched) result.Add(position);
            }

            return result;
        }

        private static List<List<char>> TranslateBitmap(List<List<char>> bitmap, int orientation)
        {
            bitmap = bitmap.ToList();
            var rotations = orientation % 4;
            for (var r = 0; r < rotations; r++)
            {
                var temp = bitmap.Select(line => new List<char>()).ToList();

                foreach (var line in bitmap)
                {
                    foreach (var c in line.Zip(temp))
                    {
                        c.Second.Insert(0, c.First);
                    }
                }

                bitmap = temp;
            }

            if (orientation >= 4)
            {
                bitmap.Reverse();
            }

            return bitmap;
        }

        private static long Run(List<Tile> state, int size, out InfiniteGrid<TileWithOrientation> output)
        {
            Console.WriteLine("================================");
            var first = state.First();
            var rest = state.Skip(1).ToList();
            for (var orientation = 0; orientation < 8; orientation++)
            {
                var grid = Search(rest, new InfiniteGrid<TileWithOrientation>
                {
                    [Position.Zero] = new TileWithOrientation(first, orientation)
                }, size);
                if (grid == null) continue;
                var (top, right, bottom, left) = (grid.Top, grid.Right, grid.Bottom, grid.Left);
                output = grid;
                return grid[new Position(left, top)].Tile.Id *
                       grid[new Position(left, bottom)].Tile.Id *
                       grid[new Position(right, top)].Tile.Id *
                       grid[new Position(right, bottom)].Tile.Id;
            }
            throw new ApplicationException();
        }

        private static InfiniteGrid<TileWithOrientation>? Search(List<Tile> tiles,
            InfiniteGrid<TileWithOrientation> grid, int size)
        {
            if (grid.Width > size || grid.Height > size) return null;

            if (grid.Width == size && grid.Height == size && grid.Positions().Count() == size * size)
            {
                return grid;
            }

            var openPositions = grid.Positions()
                .SelectMany(p => p.Orthogonal())
                .Where(p => !grid.Contains(p));

            foreach (var position in openPositions)
            {
                var constraints = new List<FacingAndValue> ();
                foreach (var facing in new[]{Vector.North, Vector.East, Vector.South, Vector.West}.Zip(new[] {0, 1, 2, 3}))
                {
                    if (grid.TryPosition(position + facing.First, out var borderingCell))
                    {
                        var borderingCellsFacing = facing.Second switch
                        {
                            0 => 2,
                            1 => 3,
                            2 => 0,
                            3 => 1,
                            _ => throw new ApplicationException()
                        };
                        constraints.Add(new FacingAndValue(facing: facing.Second, borderingCell.Borders[borderingCellsFacing]));
                    }
                }

                var validTiles = new List<TileWithOrientation>();
                foreach (var tile in tiles)
                {
                    var c = constraints.First();
                    if (!tile.Orientations.TryGetValue(c, out var proposed)) continue;
                    foreach (var constraint in constraints.Skip(1))
                    {
                        if (!tile.Orientations.TryGetValue(constraint, out var proposed2) ||
                            proposed != proposed2)
                        {
                            proposed = null;
                            break;
                        }
                    }

                    if (proposed == null) break;
                    validTiles.Add(proposed);
                }

                foreach (var tile in validTiles)
                {
                    var gridCopy = grid.Clone();
                    gridCopy.Add(position, tile);
                    var tilesCopy = tiles.ToList();
                    tilesCopy.Remove(tile.Tile);
                    var newGrid = Search(tilesCopy, gridCopy, size);
                    if (newGrid != null) return newGrid;
                }
            }

            return null;
        }

        private static void Visualize(InfiniteGrid<TileWithOrientation> grid)
        {
            for (var y = grid.Top; y <= grid.Bottom; ++y)
            {
                for (var x = grid.Left; x <= grid.Right; ++x)
                {
                    Console.Write(grid.TryPosition(new Position(x,y), out var cell) ? cell.Tile.GridId.ToString() : ".");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static List<Tile> ConvertInput(string input)
        {
            var paragraphs = input.SplitIntoParagraphs();
            var gridId = 0;
            return paragraphs.Select(p =>
            {
                var id = RegexUtils.Deserialize(p.First(), @"(?<id>\d+)", new {id = 0L}).id;

                var bitmap = p.Skip(1).Select(it => it.ToList()).ToList();
                bitmap.Should().HaveCount(10);
                bitmap.All(it => it.Count == 10).Should().BeTrue();

                return new Tile(bitmap, id, gridId++);
            })
            .ToList();
        }

        public static List<List<char>> RemoveEdges(List<List<char>> bitmap)
        {
            return bitmap.Skip(1).Take(bitmap.Count - 2)
                .Select(line => line.Skip(1).Take(line.Count - 2).ToList())
                .ToList();
        }

        public static List<List<char>> AddInColumn(List<List<char>> bitmap1, List<List<char>> bitmap2)
        {
            return bitmap1.Zip(bitmap2).Select(it =>
            {
                var temp = it.First.ToList();
                temp.AddRange(it.Second);
                return temp;
            })
            .ToList();
        }

        public static List<List<char>> AddInRows(List<List<char>> bitmap1, List<List<char>> bitmap2)
        {
            var result = bitmap1.ToList();
            result.AddRange(bitmap2);
            return result;
        }

        internal class Tile
        {
            public readonly long Id;
            public readonly long GridId;
            public readonly int[] Edges;
            public readonly List<List<char>> Bitmap;
            public readonly Dictionary<FacingAndValue, TileWithOrientation> Orientations = new Dictionary<FacingAndValue, TileWithOrientation>();

            public Tile(IEnumerable<int> edges, long id, long gridId)
            {
                Id = id;
                GridId = gridId;
                Edges = edges.ToArray();
                Bitmap = new List<List<char>>();
                for (var orientation = 0; orientation < 8; orientation++)
                {
                    var item = new TileWithOrientation(this, orientation);
                    Orientations.Add(new FacingAndValue(0, item.North), item);
                    Orientations.Add(new FacingAndValue(1, item.East), item);
                    Orientations.Add(new FacingAndValue(2, item.South), item);
                    Orientations.Add(new FacingAndValue(3, item.West), item);
                }
            }

            public Tile(List<List<char>> bitmap, long id, long gridId)
            {
                Bitmap = bitmap;
                Id = id;
                GridId = gridId;
                Edges = FindEdges(bitmap);

                for (var orientation = 0; orientation < 8; orientation++)
                {
                    var item = new TileWithOrientation(this, orientation);
                    Orientations.Add(new FacingAndValue(facing: 0, item.North), item);
                    Orientations.Add(new FacingAndValue(facing: 1, item.East), item);
                    Orientations.Add(new FacingAndValue(facing: 2, item.South), item);
                    Orientations.Add(new FacingAndValue(facing: 3, item.West), item);
                }
            }

            private static int[] FindEdges(List<List<char>> bitmap)
            {
                var north = BitmapToInt(bitmap.First());
                var south = BitmapToInt(bitmap.Last());
                var east = BitmapToInt(bitmap.Select(it => it.Last()));
                var west = BitmapToInt(bitmap.Select(it => it.First()));

                return new[] { north, east, south, west };
            }

            private static int BitmapToInt(IEnumerable<char> incoming)
            {
                var binary = incoming.Select(c => c switch
                {
                    '#' => '1',
                    '.' => '0',
                    _ => throw new ApplicationException()
                }).Join("");

                return Convert.ToInt32(binary, 2);
            }
        }

        internal class FacingAndValue
        {
            public readonly int Facing;
            public readonly int Value;

            public FacingAndValue(int facing, int value)
            {
                Facing = facing;
                Value = value;
            }

            public override bool Equals(object? obj)
            {
                if (obj == this) return true;
                return obj is FacingAndValue fv && fv.Facing == Facing && fv.Value == Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Facing, Value);
            }
        }

        internal class TileWithOrientation
        {
            public readonly Tile Tile;
            private readonly int[] TranslatedEdges;
            private readonly int Orientation;

            public readonly int North;
            public readonly int South;
            public readonly int East;
            public readonly int West;
            public readonly int[] Borders;
            public List<List<char>> Bitmap => TranslateBitmap(Tile.Bitmap, Orientation);

            public TileWithOrientation(Tile tile, int orientation)
            {
                Tile = tile;
                Orientation = orientation;
                TranslatedEdges = Tile.Edges.ToArray();
                RotateAndFlip();

                North = TranslatedEdges[0];
                East = TranslatedEdges[1];
                South = TranslatedEdges[2];
                West = TranslatedEdges[3];

                Borders = new[] { North, East, South, West }.ToArray();
            }

            private void RotateAndFlip()
            {
                var rotations = Orientation % 4;
                for (var i = 0; i < rotations; i++)
                {
                    (TranslatedEdges[0], TranslatedEdges[1], TranslatedEdges[2], TranslatedEdges[3]) =
                        (Invert(TranslatedEdges[3]), TranslatedEdges[0], Invert(TranslatedEdges[1]), TranslatedEdges[2]);
                }

                if (Orientation >= 4)
                {
                    (TranslatedEdges[0], TranslatedEdges[2]) = (TranslatedEdges[2], TranslatedEdges[0]);
                    TranslatedEdges[1] = Invert(TranslatedEdges[1]);
                    TranslatedEdges[3] = Invert(TranslatedEdges[3]);
                }
            }

            public static int Invert(int number)
            {
                var binary = (Enumerable.Repeat('0', 10).Join("") + Convert.ToString(number, 2)).Right(10);
                return Convert.ToInt32(binary.Reverse().Join(""), 2);
            }
        }
    }
}