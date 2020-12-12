namespace AdventOfCode2020.Utils
{
    public class Vector
    {
        public int dX { get; }
        public int dY { get; }

        public Vector(int dx, int dy)
        {
            dX = dx;
            dY = dy;
        }

        public static Vector operator+(Vector a, Vector b)
        {
            return new Vector(a.dX + b.dX, a.dY + b.dY);
        }

        public static Vector operator*(Vector a, int magnitude)
        {
            return new Vector(a.dX * magnitude, a.dY * magnitude);
        }
    }
}