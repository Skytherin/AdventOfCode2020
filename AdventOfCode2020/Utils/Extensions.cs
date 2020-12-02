using System.Collections.Generic;

namespace AdventOfCode2020.Utils
{
    public static class Extensions
    {
        public static bool IsInRange(this int self, int min, int max)
        {
            return min <= self && self <= max;
        }
    }
}