using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day25
    {
        private const int DoorLoopSize = 8_229_037;
        private const long DoorPublicKey = 16426071;
        private const int KeyLoopSize = 13_207_740;
        private const int KeyPublicKey = 2069194;
        private const long Modulus = 20201227;

        public static void Run()
        {
            DecryptKeys(7, 17807724, 5764801, DoorPublicKey, KeyPublicKey).Should()
                .BeEquivalentTo(11, 8, DoorLoopSize, KeyLoopSize);

            Transform(DoorPublicKey, KeyLoopSize).Should().Be(11576351L);
        }

        public static List<long> DecryptKeys(long subjectNumber, params long[] keys)
        {
            var d = new Dictionary<long, long>();
            var value = 1L;
            
            for (var loopCount = 1; loopCount <= 100_000_000; loopCount++)
            {
                value = (value * subjectNumber) % Modulus;
                if (keys.Contains(value))
                {
                    d[value] = loopCount;
                    if (keys.Length == d.Count) break;
                }
            }

            if (keys.Length != d.Count) throw new ApplicationException();
            return d.Select(kv => new {kv.Value, index = keys.ToList().IndexOf(kv.Key)})
                .OrderBy(it => it.index)
                .Select(it => it.Value)
                .ToList();
        }

        public static long Transform(long subjectNumber, int loopSize)
        {
            var value = 1L;
            var wrapCount = 0L;
            for (var i = 0; i < loopSize; i++)
            {
                var temp = value * subjectNumber;
                if (temp >= Modulus) wrapCount++;
                value = (value * subjectNumber) % Modulus;
            }

            return value;
        }

        private static long MultiplicativeInverse(long a, long b)
        {
            var n = 1;
            while ((a * n) % b != 1) ++n;
            return n;
        }
    }
}