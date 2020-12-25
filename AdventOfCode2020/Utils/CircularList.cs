using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Utils
{
    public class CircularList<T> : IEnumerable<T> where T: notnull
    {
        public readonly T Value;
        public CircularList<T> Next;
        private CircularList<T> Previous;

        private CircularList(T value)
        {
            Value = value;
            Next = this;
            Previous = this;
        }

        public static CircularList<T> From(IEnumerable<T> enumerableItems)
        {
            var items = enumerableItems.ToList();
            if (!items.Any()) throw new ApplicationException("CircularList cannot be empty");
            var first = new CircularList<T>(items.First());
            foreach(var item in items.Skip(1)) first.Add(item);
            return first;
        }

        public void Add(T value)
        {
            var newNode = new CircularList<T>(value)
            {
                Next = this, 
                Previous = Previous
            };

            Previous.Next = newNode;
            Previous = newNode;
        }

        public CircularList<T> AddRange(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                Add(value);
            }

            return this;
        }

        public IEnumerable<CircularList<T>> Walk()
        {
            yield return this;
            var current = Next;
            while (current != this)
            {
                yield return current;
                current = current.Next;
            }
        }

        public CircularList<T> Extract(int n)
        {
            if (n <= 0) throw new ApplicationException();

            var last = this;
            while (--n > 0)
            {
                if (last.Next == this) break;
                last = last.Next;
            }

            Previous.Next = last.Next;
            last.Next.Previous = Previous;

            Previous = last;
            last.Next = this;

            return this;
        }

        public void Insert(CircularList<T> chain)
        {
            var oldNext = Next;
            var oldChainPrevious = chain.Previous;
            Next = chain;
            chain.Previous.Next = oldNext;
            chain.Previous = this;
            oldNext.Previous = oldChainPrevious;
        }

        public CircularList<T> Copy()
        {
            return From(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Walk().Select(it => it.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}