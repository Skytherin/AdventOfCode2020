using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using AdventOfCode2020.Utils;

namespace AdventOfCode2020
{
    public class Day07
    {
        private static readonly List<Bag> Input = ConvertToBags(File.ReadAllText("Inputs/Day07.txt"));

        public static void Run()
        {
            Part1();
            Part2();
        }

        public static void Part1()
        {
            RunAlgorithm1(ConvertToBags(@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.")).Should().Be(4);

            RunAlgorithm1(Input).Should().Be(272);
        }

        public static void Part2()
        {
            RunAlgorithm2(ConvertToBags(@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.")).Should().Be(32);

            RunAlgorithm2(ConvertToBags(@"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags.")).Should().Be(126);

            RunAlgorithm2(Input).Should().Be(172246);
        }

        private static int RunAlgorithm2(List<Bag> bagSpace)
        {
            return CountSubBags("shiny gold", bagSpace) - 1;
        }

        private static int CountSubBags(string needle, List<Bag> bagSpace)
        {
            var bag = bagSpace.Single(it => it.Id == needle);
            return bag.SubBags.Aggregate(1, (accum, item) => accum + item.Value * CountSubBags(item.Key, bagSpace));
        }

        private static int RunAlgorithm1(List<Bag> bagSpace)
        {
            return GetAllParents("shiny gold", bagSpace).Count;
        }

        private static HashSet<string> GetAllParents(string bagId, List<Bag> bagSpace)
        {
            var parents = bagSpace
                .Where(bag => bag.SubBags.ContainsKey(bagId))
                .Select(it => it.Id)
                .ToHashSet();
            foreach (var parentHash in parents.Select(parent => GetAllParents(parent, bagSpace)).ToList())
            {
                parents.UnionWith(parentHash);
            }
            
            return parents;
        }

        private static List<Bag> ConvertToBags(string input)
        {
            return input
                .SplitIntoLines()
                .Select(line =>
                {
                    var matched =
                        RegexUtils.Deserialize<BagMatch>(line, @"^(?<Id>\w+ \w+) bags contain (?<MoreBags>.*)\.$");
                    var bag = new Bag(matched.Id);

                    foreach (var subbag in RegexUtils.DeserializeMany<MoreBagsMatch>(matched.MoreBags,
                        @"(?<Count>\d+) (?<Id>\w+ \w+) bag"))
                    {
                        bag.Add(subbag.Id, subbag.Count);
                    }

                    return bag;
                })
                .ToList();
        }
    }

    public class BagMatch
    {
        public string Id { get; set; } = "";
        public string MoreBags { get; set; } = "";
    }

    public class MoreBagsMatch
    {
        public int Count { get; set; }
        public string Id { get; set; } = "";
    }

    public class Bag
    {
        public Bag(string id)
        {
            Id = id;
        }

        public void Add(string id, int count)
        {
            SubBags.Add(id, count);
        }

        public string Id { get; set; }
        public readonly Dictionary<string, int> SubBags = new Dictionary<string, int>();
    }
}