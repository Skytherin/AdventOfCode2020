using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day14
    {
        private static readonly List<Day14Input> Input = ConvertInput(File.ReadAllText("Inputs/Day14.txt"));
        private static readonly List<Day14Input> Sample = ConvertInput(@"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0");
        private static readonly List<Day14Input> Sample2 = ConvertInput(@"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1");

        public static void Run()
        {
            var timer = new MyTimer();
            Part1(Sample).Should().Be(165);
            Part1(Input).Should().Be(5902420735773L); 

            timer.Lap();

            Part2(Sample2).Should().Be(208);
            Part2(Input).Should().Be(3801988250775L);

            timer.Lap();
            timer.Total();
        }

        private static long Part1(List<Day14Input> instructions)
        {
            var memory = new Dictionary<int, long>();
            var maskAnd = long.MaxValue;
            var maskOr = 0L;
            foreach (var instruction in instructions)
            {
                if (!string.IsNullOrWhiteSpace(instruction.Mask))
                {
                    maskAnd = long.MaxValue;
                    maskOr = 0;
                    foreach (var (x, index) in instruction.Mask.Reverse().Select((x,index)=>(x,index)))
                    {
                        if (x == '1')
                        {
                            maskOr |= (1L << index);
                        }

                        if (x == '0')
                        {
                            maskAnd &= ~(1L << index);
                        }
                    }
                }
                else if (instruction.MemAddress is {} memAddress && instruction.MemValue is {} memValue)
                {
                    memory[memAddress] = memValue & maskAnd | maskOr;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return memory.Sum(it => (long)it.Value);
        }

        private static long Part2(List<Day14Input> instructions)
        {
            var d = new FloatingDictionary();
            var currentMask = "";
            foreach (var instruction in instructions)
            {
                if (!string.IsNullOrWhiteSpace(instruction.Mask))
                {
                    currentMask = instruction.Mask;
                }
                else if (instruction.MemAddress is { } memAddress && instruction.MemValue is { } memValue)
                {
                    d.Add(memAddress, currentMask, memValue);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return d.Sum();
        }

        private static List<Day14Input> ConvertInput(string input)
        {
            return input.SplitIntoLines()
                .Select(line => RegexUtils.Deserialize<Day14Input>(line, @"(mask = (?<Mask>[X0-9]+))|(mem\[(?<MemAddress>\d+)] = (?<MemValue>\d+))"))
                .ToList();
        }

        public class Day14Input
        {
            public string? Mask { get; set; }
            public int? MemAddress { get; set; }
            public long? MemValue { get; set; }
        }
    }

    public class FloatingDictionary
    {
        private readonly Dictionary<string, long> Actual = new Dictionary<string, long>();

        public void Add(long address, string mask, long value)
        {
            var keyArray = (Enumerable.Repeat("0", 64).Join("") + Convert.ToString(address, 2)).Right(64).Reverse()
                .ToArray();
            foreach (var (c, index) in mask.Reverse().Select((c, index) => (c, index)))
            {
                if (c == '0') continue;
                if (c == '1') keyArray[index] = '1';
                else if (c == 'X') keyArray[index] = 'X';
                else throw new ApplicationException();
            }

            var key = keyArray.Join("");
            key.Length.Should().Be(64);

            SplitOverlappedKeys(key);
            RemoveSubsumedKeys(key);

            Actual.Add(key, value);
        }

        public long Sum()
        {
            return Actual.Sum(kv => kv.Value * Pow2(kv.Key.Count(c => c == 'X')));
        }

        private static long Pow2(int n)
        {
            return Enumerable.Repeat(2, n).Aggregate(1, (prev, current) => prev * current);
        }

        private void RemoveSubsumedKeys(string key)
        {
            var removedKeys = Actual.Keys.Where(needle => Subsumed(needle, key)).ToList();
            foreach (var removedKey in removedKeys)
            {
                Actual.Remove(removedKey);
            }
        }

        private bool Subsumed(string existingKey, string newKey)
        {
            var subsumed = false;
            foreach (var (n, k) in existingKey.Zip(newKey))
            {
                if (k == 'X')
                {
                    subsumed = true;
                }
                else if (n != k)
                {
                    return false;
                }
            }

            return subsumed;
        }

        private void SplitOverlappedKeys(string newKey)
        {
            var repeat = true;
            while (repeat)
            {
                repeat = false;
                foreach (var key in Actual.Keys.ToList())
                {
                    // skip this key if it's non-X's dont match new non-Xs
                    if (!SplitKey(key, newKey))
                    {
                        continue;
                    }
                    repeat = true;
                }
            }
        }

        private bool SplitKey(string existingKey, string newKey)
        {
            // Split existing keys when the existing key has an X
            // where the newKey has a 1 or 0
            var splitLocations = new List<int>();
            for (var i =0; i < existingKey.Length; ++i)
            {
                var n = newKey[i];
                var e = existingKey[i];
                if (n == 'X') continue;
                if (e == 'X')
                {
                    splitLocations.Add(i);
                }
                else if (e != n)
                {
                    return false;
                }
            }

            if (splitLocations.Count == 0) return false;
            var splits = CreateSplits(existingKey, 0, splitLocations);
            foreach (var split in splits)
            {
                Actual[split] = Actual[existingKey];
            }

            Actual.Remove(existingKey);
            return true;
        }

        private IEnumerable<string> CreateSplits(string existingKey, int offset, List<int> splitLocations)
        {
            if (splitLocations.Count == 0)
            {
                yield return existingKey.Substring(offset);
                yield break;
            }

            var location = splitLocations.First();
            var first = existingKey.Substring(offset, location - offset);
            var remainders = CreateSplits(existingKey, 
                location + 1,
                splitLocations.Skip(1).ToList());
            foreach (var remainder in remainders)
            {
                yield return first + '0' + remainder;
                yield return first + '1' + remainder;
            }
        }
    }
}