using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Utils
{
    public class CircularList<T>
    {
        public readonly T Value;

        public CircularList<T> Next;
        private CircularList<T> Previous;

        public CircularList(T value)
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

        public IEnumerable<T> ReverseEnumerate()
        {
            yield return Previous.Value;
            var current = Previous;
            while (current != this)
            {
                yield return current.Previous.Value;
                current = current.Previous;
            }
        }

        public IEnumerable<T> Enumerate()
        {
            yield return Value;
            var current = Next;
            while (current != this)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        public CircularList<T>? Find(Func<T, bool> needle)
        {
            if (needle(Value)) return this;
            var current = Next;
            while (current != this)
            {
                if (needle(current.Value)) return current;
                current = current.Next;
            }

            return null;
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
            return From(Enumerate());
        }
    }
}