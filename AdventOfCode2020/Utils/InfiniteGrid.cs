using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Utils
{
    public class InfiniteGrid<T>
    {
        public int Top => Cells().Select(it => it.position.Y).Min();
        public int Bottom => Cells().Select(it => it.position.Y).Max();
        public int Left => Cells().Select(it => it.position.X).Min();
        public int Right => Cells().Select(it => it.position.X).Max();

        public int Width => WidthWith(Cells().Select(it => it.position));
        public int Height => HeightWith(Cells().Select(it => it.position));

        private int WidthWith(IEnumerable<Position> positions)
        {
            var items = positions.Aggregate(new {left = (int?)0, right = (int?)0}, (previous, position) => new
            {
                left = (int?)Math.Min(position.X, previous.left ?? position.X),
                right = (int?)Math.Max(position.X, previous.right ?? position.X)
            });

            return (items.right ?? 0) - (items.left ?? 0) + 1;
        }

        private int HeightWith(IEnumerable<Position> positions)
        {
            var items = positions.Aggregate(new { top = (int?)0, bottom = (int?)0 }, (previous, position) => new
            {
                top = (int?)Math.Min(position.Y, previous.top ?? position.X),
                bottom = (int?)Math.Max(position.Y, previous.bottom ?? position.X)
            });

            return (items.bottom ?? 0) - (items.top ?? 0) + 1;
        }

        public int WidthWith(Position p) => WidthWith(Cells().Select(it => it.position).Append(p));
        public int HeightWith(Position p) => HeightWith(Cells().Select(it => it.position).Append(p));

        private readonly Dictionary<Position, T> Actual = new Dictionary<Position, T>();

        public IEnumerable<(Position position, T value)> Cells() =>
            Actual.Select(kv => (position: kv.Key, kv.Value));

        public T this[Position key]
        {
            get => Actual[key];
            set => Actual[key] = value;
        }

        public IEnumerable<Position> Positions() => Actual.Keys;

        public bool Contains(Position position)
        {
            return Actual.ContainsKey(position);
        }

        public InfiniteGrid<T> Clone()
        {
            var result = new InfiniteGrid<T>();
            foreach (var (key, value) in Actual)
            {
                result[key] = value;
            }

            return result;
        }

        public bool TryPosition(Position position, out T value)
        {
            return Actual.TryGetValue(position, out value!);
        }

        public void Add(Position position, T value)
        {
            Actual.Add(position, value);
        }
    }

    public static class InfiniteGridExtensions
    {
        public static T ValueOrDefault<T>(this InfiniteGrid<T> self, Position key)
        {
            return self.TryPosition(key, out var value) ? value : default!;
        }
    }
}