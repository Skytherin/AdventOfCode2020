using System.Collections.Generic;

namespace AdventOfCode2020.Utils
{
    public class SummedList
    {
        public long Sum { get; private set; }
        private readonly List<long> Actual = new List<long>();
        public IReadOnlyList<long> List => Actual;
        public int Count => Actual.Count;

        public void Add(long item)
        {
            Sum += item;
            Actual.Add(item);
        }

        public void Shift()
        {
            var value = Actual.Shift();
            Sum -= value;
        }
    }
}