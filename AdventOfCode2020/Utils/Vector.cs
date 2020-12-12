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

        public Vector Add(Vector vector, int magnitude)
        {
            return new Vector(dX + vector.dX * magnitude, dY + vector.dY * magnitude);
        }

    }
}