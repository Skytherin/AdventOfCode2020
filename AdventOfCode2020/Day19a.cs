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
    public static class Day19a
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

            //Run(sample1).Should().Be(2);
            //Run(sample2).Should().Be(2);
            //Run(input).Should().Be(120);

            timer.Lap();

            var sample3 = ConvertInput(@"0: 1 | 2
1: 0 2
2: ""a""

a
aa
aaa");

            var sample5 = ConvertInput(@"0: 11 | 8
8: 42 | 42 8
11: 42 31 | 42 11 31
42: ""a""
31: ""b""

ab
aabb
aaabbb
aaaabbbb");

            var sample4 = ConvertInput(@"42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: ""a""
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: ""b""
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba");

            //Run(sample3).Should().Be(3);
            //Run(sample4).Should().Be(3);
            Run(sample5).Should().Be(4);
            Run2(sample4).Should().Be(12);
            Run2(input).Should().Be(350);

            timer.Lap();
            timer.Total();
        }

        private static long Run(Day19Input state)
        {
            // Turn rules into regex
            //var regex = RulesToRegex(0, state.Nodes);
            var rx = new  Dictionary<int, string>();
            RulesToRegex2(0, state, new List<int>(), rx, new Dictionary<int, HashSet<int>>(), new HashSet<int>());
            var regex = rx[0];
            regex = $"^{regex}$";
            return state.Messages.Count(message =>
            {
                var m = Regex.IsMatch(message, regex);
                if (!m) Console.WriteLine(message);
                return m;
            });
        }

        private static long Run2(Day19Input state)
        {
            CreateRule(state, 8, "42 | 42 8");
            CreateRule(state, 11, "42 31 | 42 11 31");
            return Run(state);
        }

        private static void RulesToRegex2(int node,
            Day19Input state,
            List<int> pathSoFar, 
            Dictionary<int, string> regex,
            Dictionary<int, HashSet<int>> loops,
            HashSet<int> unresolved)
        {
            if (regex.ContainsKey(node))
            {
                return;
            }

            if (state.Lexes.TryGetValue(node, out var lex))
            {
                regex[node] = lex.ToString();
                return;
            }

            pathSoFar = pathSoFar.ToList();
            pathSoFar.Add(node);

            var result = new List<string>();
            var variants = state.Rules[node];
            if (node == 8)
            {
                variants = variants.Take(1).ToList();
            }
            else if (node == 11)
            {
                variants = new List<List<int>>
                {
                    new List<int>
                    {
                        42,
                        31
                    }
                };
            }
            foreach (var variant in variants)
            {
                foreach (var childNode in variant)
                {
                    var loopIndex = pathSoFar.IndexOf(childNode);
                    if (loopIndex != -1)
                    {
                        loops.Add(pathSoFar.Last(), childNode);
                        continue;
                    }
                    RulesToRegex2(childNode, state, pathSoFar, regex, loops, unresolved);
                }

                var variantRegex = "";
                bool tailRecursive = node == 8;
                var rules = variant.Select((it, index) => (it, index)).ToList();

                if (node == 11)
                {
                    var r42 = regex[42];
                    var r31 = regex[31];
                    for (int i = 1; i < 10; i++)
                    {
                        if (i > 1) variantRegex += "|";
                        variantRegex += "(";
                        variantRegex += $"({r42}){{{i}}}";
                        variantRegex += $"({r31}){{{i}}}";
                        variantRegex += ")";
                    }
                    variantRegex = $"({variantRegex})";
                }
                else
                {
                    foreach (var rule in rules)
                    {
                        var childNode = rule.it;
                        if (!regex.ContainsKey(childNode))
                        {
                            variantRegex += $"<{childNode}>";
                            unresolved.Add(childNode);
                            continue;
                        }
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

                    if (tailRecursive)
                    {
                        variantRegex = $"({variantRegex})+";
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

            if (unresolved.Contains(node))
            {
                foreach (var key in regex.Keys.ToList())
                {
                    var k = $"<{node}>";
                    if (regex[key].Contains(k))
                    {
                        if (loops.ContainsKey(node))
                        {
                            regex[key] = regex[key].Replace(k, $"({regex[node]})*");
                        }
                        else
                        {
                            regex[key] = regex[key].Replace(k, $"({regex[node]})");
                        }
                    }
                }

                unresolved.Remove(node);
            }
        }


        private static string RulesToRegex(int startNode, Day19Input state)
        {
            var result = new List<string>();
            var current = state.Rules[startNode];
            foreach (var variant in current)
            {
                result.Add(variant.Select(v =>
                {
                    if (state.Lexes.TryGetValue(v, out var lex))
                    {
                        return $"{lex}";
                    }

                    return RulesToRegex(v, state);
                }).Join(""));
            }

            if (result.Count == 1) return result.First();

            return $"({result.Select(v => $"{v}").Join("|")})";
        }

        private static void CreateRule(Day19Input input, int name, string rules)
        {
            var result = new List<List<int>>();
            var variants = rules.Split("|")
                .Select(group => group.Trim().Split(" ").Select(it => it.Trim()).ToList())
                .ToList();
            
            if (variants.Count == 1 && variants.First().Count == 1)
            {
                var item = variants.First().First();
                if (Regex.IsMatch(item, @"""[ab]"""))
                {
                    input.Lexes[name] = item[1];
                }
                return;
            }
            
            foreach (var variant in variants)
            {
                var v = new List<int>();
                result.Add(v);
                foreach (var item in variant)
                {
                    if (Regex.IsMatch(item, @"\d+"))
                    {
                        v.Add(Convert.ToInt32(item));
                    }
                    else throw new ApplicationException();
                }
            }

            input.Rules[name] = result;
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
                CreateRule(result, name, rule);
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
        public readonly Dictionary<int, char> Lexes = new Dictionary<int, char>();
        public readonly Dictionary<int, List<List<int>>> Rules = new Dictionary<int, List<List<int>>>();
        public readonly List<string> Messages = new List<string>();
    }
}