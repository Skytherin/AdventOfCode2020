using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Utils
{
    public class DictionaryWithDefault<TKey, TValue>: IDictionary<TKey, TValue> where TKey: notnull
    {
        private readonly Dictionary<TKey, TValue> Actual = new Dictionary<TKey, TValue>();
        private readonly TValue Default;

        public DictionaryWithDefault(TValue @default)
        {
            Default = @default;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Actual.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Actual.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Actual.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Actual.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kv in Actual)
            {
                array[arrayIndex++] = kv;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count => Actual.Count;
        public bool IsReadOnly => false;
        public void Add(TKey key, TValue value)
        {
            Actual.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return Actual.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return Actual.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = Actual.TryGetValue(key, out var value2) ? value2 : Default;
            return true;
        }

        public TValue this[TKey key]
        {
            get => Actual.TryGetValue(key, out var value2) ? value2 : Default;
            set => Actual[key] = value;
        }

        public ICollection<TKey> Keys => Actual.Keys;
        public ICollection<TValue> Values => Actual.Values;
    }
}