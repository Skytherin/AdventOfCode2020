using System;
using System.Collections;
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

        private static long Run(DictionaryWithDefault<PositionNd, CubeState> initialState)
        {
            // Visualize(initialState);
            return Produce.Iterate(initialState, 6, current =>
                {
                    var next = new DictionaryWithDefault<PositionNd, CubeState>(CubeState.Inactive);
                    var adjacentCounts = new DictionaryWithDefault<PositionNd, int>(0);

                    // Look at all active cells and all cells adjacent to active cells
                    var cells = new HashSet<PositionNd>();
                    foreach (var (position, _) in current.Where(kv => kv.Value == CubeState.Active))
                    {
                        cells.Add(position);
                        cells.UnionWith(position.Adjacents());
                        foreach (var pos2 in position.Adjacents())
                        {
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

        private static void Visualize(DictionaryWithDefault<PositionNd, CubeState> next)
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
                        s += next[new PositionNd(x, y, z)] switch
                        {
                            CubeState.Active => "#",
                            _ => "."
                        };
                    }
                    Console.WriteLine(s);
                }
            }
        }

        private static DictionaryWithDefault<PositionNd, CubeState> ConvertInput(string input, int dimensionality)
        {
            var result = new DictionaryWithDefault<PositionNd, CubeState>(CubeState.Inactive);
            foreach (var (line,row) in input.SplitIntoLines().Select((line, row) => (line, row)))
            {
                foreach (var (c,col) in line.Select((c, col) => (c, col)))
                {
                    result[new PositionNd(col, row).SetDimensionality(dimensionality)] = 
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

    public class DictionaryWithDefault<TKey, TValue>: IDictionary<TKey, TValue> where TKey: notnull
    {
        private readonly Dictionary<TKey, TValue> Actual = new Dictionary<TKey, TValue>();
        private readonly TValue Default;

        public DictionaryWithDefault(TValue @default)
        {
            Default = @default;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Actual.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Actual.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Actual.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Actual.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kv in Actual)
            {
                array[arrayIndex++] = kv;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count => Actual.Count;
        public bool IsReadOnly => false;
        public void Add(TKey key, TValue value)
        {
            Actual.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return Actual.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return Actual.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = Actual.TryGetValue(key, out var value2) ? value2 : Default;
            return true;
        }

        public TValue this[TKey key]
        {
            get => Actual.TryGetValue(key, out var value2) ? value2 : Default;
            set => Actual[key] = value;
        }

        public ICollection<TKey> Keys => Actual.Keys;
        public ICollection<TValue> Values => Actual.Values;
    }

    public class PositionNd
    {
        public readonly List<int> Positions;
        private readonly int HashCode;

        public PositionNd(params int[] positions)
        {
            Positions = positions.ToList();
            HashCode = Positions.Aggregate((accum, current) => accum.HashWith(current));
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public override bool Equals(object? obj)
        {
            if (obj == this) return true;
            return obj is PositionNd other && 
                   Positions.Count == other.Positions.Count && 
                   Positions.Zip(other.Positions).All(item => item.First == item.Second);
        }

        public IEnumerable<PositionNd> Adjacents()
        {
            var ns = Enumerable.Repeat(-1, Positions.Count).ToArray();
            yield return new PositionNd(Positions.Zip(ns).Select(it => it.First + it.Second).ToArray());
            while (true)
            {
                var i = 0;
                ns[i] += 1;
                while (ns[i] >= 2)
                {
                    ns[i] = -1;
                    i += 1;
                    if (i >= ns.Length) yield break;
                    ns[i] += 1;
                }
                if (ns.All(n => n == 0)) continue;
                yield return new PositionNd(Positions.Zip(ns).Select(it => it.First + it.Second).ToArray());
            }
        }

        public PositionNd SetDimensionality(int dimensionality)
        {
            var positions = Positions.ToList();
            while (positions.Count < dimensionality)
            {
                positions.Add(0);
            }
            return new PositionNd(positions.ToArray());
        }
    }
}