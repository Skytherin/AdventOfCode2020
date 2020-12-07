using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using AdventOfCode2020.Utils;

namespace AdventOfCode2020
{
    public class Day7
    {
        private static readonly Dictionary<string, Bag> Input = ConvertToBags(File.ReadAllText("Inputs/Day7.txt"));

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

        private static int RunAlgorithm2(Dictionary<string, Bag> bagSpace)
        {
            return CountSubBags("shiny gold", bagSpace) - 1;
        }

        private static int CountSubBags(string needle, Dictionary<string, Bag> bagSpace)
        {
            return bagSpace[needle].SubBags.Aggregate(1, (accum, item) => accum + item.Value * CountSubBags(item.Key, bagSpace));
        }

        private static int RunAlgorithm1(Dictionary<string, Bag> bagSpace)
        {
            return GetAllParents("shiny gold", MapChildrenToParents(bagSpace));
            //var needle = "shiny gold";
            //var childrenToParents = MapChildrenToParents(bagSpace);

            //var closed = new HashSet<string>();
            //var open = new Queue<string>();
            //open.Enqueue(needle);

            //while (open.Any())
            //{
            //    var child = open.Dequeue();

            //    if (childrenToParents.TryGetValue(child, out var parents))
            //    {
            //        foreach (var parent in parents)
            //        {
            //            open.Enqueue(parent);
            //        }
            //    }
            //    if (bagSpace.ContainsKey(child))
            //    {
            //        closed.Add(child);
            //    }
            //}

            //closed.Remove(needle);

            //return closed.Count;
        }

        private static Dictionary<string, List<string>> MapChildrenToParents(Dictionary<string, Bag> bagSpace)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var bag in bagSpace.Values)
            {
                foreach (var subbag in bag.SubBags)
                {
                    if (result.ContainsKey(subbag.Key))
                    {
                        result[subbag.Key].Add(bag.Id);
                    }
                    else
                    {
                        result[subbag.Key] = new List<string> {bag.Id};
                    }
                }
            }

            return result;
        }

        private static Dictionary<string, Bag> ConvertToBags(string input)
        {
            return input
                .SplitIntoLines()
                .Select(line =>
                {
                    var matched =
                        RegexUtils.Deserialize<BagMatch>(line, @"^(?<Id>\w+ \w+) bags contain (?<MoreBags>.*)\.$");
                    var bag = new Bag(matched.Id);

                    if (matched.MoreBags == "no other bags") return bag;
                    var subbags = matched.MoreBags
                        .Split(",")
                        .Select(it =>
                            RegexUtils.Deserialize<MoreBagsMatch>(it.Trim(), @"(?<Count>\d+) (?<Id>\w+ \w+) bag(s?)"));
                    foreach (var subbag in subbags)
                    {
                        bag.Add(subbag.Id, subbag.Count);
                    }

                    return bag;
                })
                .ToDictionary(it => it.Id, it => it);
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