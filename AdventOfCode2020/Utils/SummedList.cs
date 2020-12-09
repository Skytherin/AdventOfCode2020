using System.Collections.Generic;

namespace AdventOfCode2020.Utils
{
    public class SummedList
    {
        public long Sum { get; private set; }
        private readonly List<long> MyList = new List<long>();
        public IReadOnlyList<long> List => MyList;
        public int Count => MyList.Count;

        public void Add(long item)
        {
            Sum += item;
            MyList.Add(item);
        }

        public void Shift()
        {
            var value = MyList.Shift();
            Sum -= value;
        }
    }
}