using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public class Day02
    {
        private static List<Day2Input> SampleInput = @"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc".Split("\n").Select(line => StringToInput(line)).ToList();

        private static List<Day2Input> Input = File.ReadAllLines("Inputs/Day02.txt").Select(line => StringToInput(line)).ToList();

        public static void Run()
        {
            Console.WriteLine("=== DAY 1 ===");
            Part1().Should().BeEquivalentTo(2, 519);
            Part2().Should().BeEquivalentTo(1, 708);
        }

        public static IEnumerable<int> Part1()
        {
            Console.WriteLine("=== PART 1 ===");
            yield return RunAlgorithm1(SampleInput);
            yield return RunAlgorithm1(Input);
        }

        public static IEnumerable<int> Part2()
        {
            Console.WriteLine("=== PART 2 ===");
            yield return RunAlgorithm2(SampleInput);
            yield return RunAlgorithm2(Input);
        }

        private static int RunAlgorithm1(IEnumerable<Day2Input> inputs)
        {
            return inputs.Count(input => input.Password.Count(c => c == input.RequiredCharacter).IsInRange(input.RangeMin, input.RangeMax));
        }

        private static int RunAlgorithm2(IEnumerable<Day2Input> inputs)
        {
            return inputs.Count(input => (input.Password.ElementAt(input.RangeMin - 1) == input.RequiredCharacter) ^
                                         (input.Password.ElementAt(input.RangeMax - 1) == input.RequiredCharacter));
        }

        private static Day2Input StringToInput(string s)
        {
            return RegexUtils.Deserialize<Day2Input>(s.Trim(),
                @"(?<RangeMin>\d+)-(?<RangeMax>\d+) (?<RequiredCharacter>.): (?<Password>.*)");
        }
    }

    public class Day2Input
    {
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }
        public char RequiredCharacter { get; set; }
        public string Password { get; set; } = default!;
    }
}