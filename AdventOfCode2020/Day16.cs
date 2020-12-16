using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day16
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day16.txt"));
            var sample = ConvertInput(@"class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12");

            var sample2 = ConvertInput(@"class: 0-1 or 4-19
row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9");

            var timer = new MyTimer();
            Part1(sample).Should().Be(71);
            Part1(input).Should().Be(18142L); 

            timer.Lap();

            GetPositions(sample2)
                .Select(it => it.Name)
                .Should()
                .BeEquivalentTo("row", "class", "seat");
            Part2(input).Should().Be(1069784384303);

            timer.Lap();
            timer.Total();
        }

        private static long Part1(Day16Input input)
        {
            return input.Tickets
                .SelectMany(ticket => ticket.Where(value => input.Classes.All(klass => !klass.Validate(value))))
                .Sum();

        }

        private static long Part2(Day16Input input)
        {
            var positionToClass = GetPositions(input).ToList();

            long sum = 1;
            foreach (var rule in input.Classes.Where(it => it.Name.StartsWith("departure")))
            {
                var positionIndex = positionToClass.Select((it, index) => (it, index)).Single(it => it.it == rule).index;
                sum *= input.MyTicket[positionIndex];
            }

            return sum;
        }

        private static IEnumerable<Day16Class> GetPositions(Day16Input input)
        {
            var tickets = input.Tickets
                .Where(ticket => ticket.All(value => input.Classes.Any(klass => klass.Validate(value))))
                .ToList();

            var rotated = tickets
                .SelectMany(ticket => ticket.Select((it,index)=>(it,index)))
                .GroupBy(it => it.index)
                .ToDictionary(it => it.Key, it => it.Select(z => z.it));

            var positionToClass = Enumerable.Range(0, tickets.First().Count)
                .Select(index => input.Classes.Where(klass => rotated[index].All(value => klass.Validate(value))).ToList())
                .ToArray();

            var changed = true;
            while (changed)
            {
                changed = false;
                for (var i = 0; i < positionToClass.Length; ++i)
                {
                    if (positionToClass[i].Count == 1)
                    {
                        var good = positionToClass[i][0];
                        for (var j = 0; j < positionToClass.Length; ++j)
                        {
                            if (i != j)
                            {
                                changed = changed || positionToClass[j].Remove(good);
                            }
                        }

                        // if (changed) break;
                    }
                }
            }

            foreach (var rules in positionToClass)
            {
                rules.Should().HaveCount(1);
            }

            return positionToClass.Select(it => it.Single());
        }

        private static Day16Input ConvertInput(string input)
        {
            var result = new Day16Input();
            var groups = input.SplitOnBlankLines();

            groups.Should().HaveCount(3);

            result.Classes.AddRange(groups[0]
                .SplitIntoLines()
                .Select(line => RegexUtils.Deserialize<Day16Class>(line,
                    @"(?<Name>[^:]+): (?<Range1Min>\d+)-(?<Range1Max>\d+) or (?<Range2Min>\d+)-(?<Range2Max>\d+)")));

            result.MyTicket =
                groups[1]
                    .SplitIntoLines()
                    .Skip(1)
                    .Select(it => it.Split(",").Select(item => Convert.ToInt32(item)).ToList())
                    .Single();

            result.Tickets.AddRange(groups[2]
                .SplitIntoLines()
                .Skip(1)
                .Select(it => it.Split(",").Select(item => Convert.ToInt32(item)).ToList()));

            return result;
        }

        public class Day16Input
        {
            public readonly List<Day16Class> Classes = new List<Day16Class>();
            public List<int> MyTicket { get; set; } = new List<int>();
            public List<List<int>> Tickets { get; set; } = new List<List<int>>();
        }

        public class Day16Class
        {
            public string Name { get; set; } = "";
            public int Range1Min { get; set; }
            public int Range1Max { get; set; }
            public int Range2Min { get; set; }
            public int Range2Max { get; set; }

            public bool Validate(int proposed)
            {
                return (Range1Min <= proposed && proposed <= Range1Max) ||
                       (Range2Min <= proposed && proposed <= Range2Max);
            }
        }
    }

}