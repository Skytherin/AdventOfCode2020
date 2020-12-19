using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day19
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day19.txt"));
            var timer = new MyTimer();

            var sample1 = ConvertInput(@"0: 1 2
1: ""a""
2: 1 3 | 3 1
3: ""b""

aab
aba");

            var sample2 = ConvertInput(@"0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: ""a""
5: ""b""

ababbb
bababa
abbbab
aaabbb
aaaabbb");

            Run(sample1).Should().Be(2);
            Run(sample2).Should().Be(2);
            Run(input).Should().Be(120);

            timer.Lap();

            timer.Lap();
            timer.Total();
        }

        private static long Run(Day19Input state)
        {
            // Turn rules into regex
            //var regex = RulesToRegex(0, state.Nodes);
            var rx = new  Dictionary<int, string>();
            RulesToRegex2(0, state.Nodes, new List<int>(), rx, new Dictionary<int, HashSet<int>>());
            var regex = rx[0];
            regex = $"^{regex}$";
            return state.Messages.Count(message => Regex.IsMatch(message, regex));
        }

        private static long Run2(Day19Input state)
        {
            state.Nodes[8] = CreateRule("42 | 42 8");
            state.Nodes[11] = CreateRule("42 31 | 42 11 31");
            // Turn rules into regex
            var regex = RulesToRegex(0, state.Nodes);
            regex = $"^{regex}$";
            return state.Messages.Count(message => Regex.IsMatch(message, regex));
        }

        private static void RulesToRegex2(int node,
            Dictionary<int, List<List<Day19Input.Node>>> stateNodes,
            List<int> pathSoFar, 
            Dictionary<int, string> regex,
            Dictionary<int, HashSet<int>> loops)
        {
            if (regex.ContainsKey(node))
            {
                return;
            }

            if (stateNodes[node].Count == 1 && stateNodes[node][0][0].Lex is {} c)
            {
                regex[node] = c.ToString();
                return;
            }

            pathSoFar = pathSoFar.ToList();
            pathSoFar.Add(node);

            var result = new List<string>();
            foreach (var variant in stateNodes[node])
            {
                foreach (var childNode in variant.Select(it => it.Rule).OfType<int>())
                {
                    var loopIndex = pathSoFar.IndexOf(childNode);
                    if (loopIndex != -1)
                    {
                        loops.Add(pathSoFar.Last(), childNode);
                        continue;
                    }
                    RulesToRegex2(childNode, stateNodes, pathSoFar, regex, loops);
                }

                var variantRegex = "";
                foreach (var childNode in variant.Select(it => it.Rule).OfType<int>())
                {
                    var thisOne = regex[childNode];
                    if (loops.ContainsKey(childNode))
                    {
                        variantRegex += $"({thisOne})*";
                    }
                    else
                    {
                        variantRegex += $"{thisOne}";
                    }
                }

                result.Add(variantRegex);
            }

            if (result.Count == 1)
            {
                regex.Add(node, result.First());
            }
            else
            {
                regex.Add(node, $"({result.Select(v => $"{v}").Join("|")})");
            }
        }


        private static string RulesToRegex(int startNode, Dictionary<int, List<List<Day19Input.Node>>> stateNodes)
        {
            var result = new List<string>();
            var current = stateNodes[startNode];
            foreach (var variant in current)
            {
                result.Add(variant.Select(v =>
                {
                    if (v.Lex != null) return $"{v.Lex}";

                    var nextRule = v.Rule ?? throw new ApplicationException();

                    return RulesToRegex(v.Rule ?? throw new ApplicationException(), stateNodes);
                }).Join(""));
            }

            if (result.Count == 1) return result.First();

            return $"({result.Select(v => $"{v}").Join("|")})";
        }

        private static List<List<Day19Input.Node>> CreateRule(string rules)
        {
            var result = new List<List<Day19Input.Node>>();
            var variants = rules.Split("|").Select(group => group.Trim().Split(" ").Select(it => it.Trim()));
            foreach (var variant in variants)
            {
                var v = new List<Day19Input.Node>();
                result.Add(v);
                foreach (var item in variant)
                {
                    if (Regex.IsMatch(item, @"\d+"))
                    {
                        v.Add(new Day19Input.Node{Rule = Convert.ToInt32(item)});
                    }
                    else if (Regex.IsMatch(item, @"""[ab]"""))
                    {
                        v.Add(new Day19Input.Node { Lex = item[1] });
                    }
                    else throw new ApplicationException();
                }
            }

            return result;
        }

        private static Day19Input ConvertInput(string input)
        {
            var result = new Day19Input();

            var g = input.SplitOnBlankLines();
            foreach (var line in g[0].SplitIntoLines())
            {
                var m1 = RegexUtils.Deserialize(line, @"(?<name>\d+): (?<rule>.*)", new {name = 0, rule = ""});
                var name = m1.name;
                var rule = m1.rule;
                result.Nodes[name] = CreateRule(rule);
            }

            foreach (var line in g[1].SplitIntoLines())
            {
                result.Messages.Add(line.Trim());
            }

            return result;
        }
    }

    public class Day19Input
    {
        public class Node
        {
            public int? Rule { get; set; }
            public char? Lex { get; set; }
        }

        public readonly Dictionary<int, List<List<Node>>> Nodes = new Dictionary<int, List<List<Node>>>();

        public readonly List<string> Messages = new List<string>();
    }
}