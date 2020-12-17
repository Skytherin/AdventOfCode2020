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
            return X.HashWith(Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is Position other && other.X == X && other.Y == Y;
        }

        public static Position operator +(Position p, Vector vector)
        {
            return new Position(p.X + vector.dX, p.Y + vector.dY);
        }

        public long ManhattanDistance()
        {
            return Math.Abs(X) + Math.Abs(Y);
        }
    }
}