using System;

namespace AdventOfCode2020.Utils
{
    public class Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return (X << 16) + (X >> 16) + Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Position other && other.X == X && other.Y == Y;
        }

        public Position Add(Vector vector, int magnitude)
        {
            return new Position(X + vector.dX * magnitude, Y + vector.dY * magnitude);
        }

        public long ManhattanDistance()
        {
            return Math.Abs(X) + Math.Abs(Y);
        }
    }
}