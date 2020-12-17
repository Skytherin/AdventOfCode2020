using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Utils
{
    public class MultiDimensionalPosition
    {
        public readonly List<int> Positions;
        private readonly int HashCode;

        public MultiDimensionalPosition(params int[] positions)
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
            return obj is MultiDimensionalPosition other && 
                   Positions.Count == other.Positions.Count && 
                   Positions.Zip(other.Positions).All(item => item.First == item.Second);
        }

        public IEnumerable<MultiDimensionalPosition> Adjacents()
        {
            var ns = Enumerable.Repeat(-1, Positions.Count).ToArray();
            yield return new MultiDimensionalPosition(Positions.Zip(ns).Select(it => it.First + it.Second).ToArray());
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
                yield return new MultiDimensionalPosition(Positions.Zip(ns).Select(it => it.First + it.Second).ToArray());
            }
        }

        public MultiDimensionalPosition SetDimensionality(int dimensionality)
        {
            var positions = Positions.ToList();
            while (positions.Count < dimensionality)
            {
                positions.Add(0);
            }
            return new MultiDimensionalPosition(positions.ToArray());
        }
    }
}