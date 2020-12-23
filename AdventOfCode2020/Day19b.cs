using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day19b
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
            timer.Lap();
            Run2(input).Should().Be(350);

            timer.Lap();
            timer.Total();
        }

        private static long Run(Day19Input state)
        {
            return state.Messages.Count(message =>
            {
                var search = new Search();

                while (message.Length > 0 && search.Stack.Any())
                {
                    var c = message[0];
                    message = message.Substring(1);
                    search = search.Advance(c, state, out var matched);
                    if (matched && message.Length == 0) return true;
                }

                return false;
            });
        }

        private static long Run2(Day19Input state)
        {
            Day19a.CreateRule(state, 8, "42 | 42 8");
            Day19a.CreateRule(state, 11, "42 31 | 42 11 31");
            return Run(state);
        }

        private static Day19Input ConvertInput(string input)
        {
            return Day19a.ConvertInput(input);
        }

        class Search
        {
            internal class Frame
            {
                public readonly IReadOnlyList<int> Variant;
                public readonly int NextRule;

                public Frame(IEnumerable<int> variant)
                {
                    Variant = variant.ToList();
                    NextRule = 0;
                }

                public Frame(IEnumerable<int> variant, int nextRule)
                {
                    Variant = variant.ToList();
                    NextRule = nextRule;
                }
            }

            public readonly List<List<Frame>> Stack = new List<List<Frame>>();

            public Search()
            {
                Stack.Add(new List<Frame>
                {
                    new Frame(new List<int>{0})
                });
            }

            private Search(List<List<Frame>> frames)
            {
                Stack = frames;
            }
            
            public Search Advance(char c, Day19Input state, out bool matched)
            {
                matched = false;
                var closed = new List<List<Frame>>();
                var open = Stack.ToList();

                while (open.Any())
                {
                    var frames = open.Shift();

                    var frame = frames.Pop();
                    var nextRule = frame.Variant[frame.NextRule];
                    frame = new Frame(frame.Variant, frame.NextRule + 1);
                    if (state.Lexes.TryGetValue(nextRule, out var lex))
                    {
                        if (lex != c) continue;

                        while (frames.Any() && frame.NextRule >= frame.Variant.Count)
                        {
                            frame = frames.Pop();
                        }

                        if (frame.NextRule < frame.Variant.Count)
                        {
                            closed.Add(frames.Append(frame).ToList());
                            continue;
                        }

                        matched = true;
                        continue;
                    }

                    var nextVariants = state.Rules[nextRule];
                    foreach (var variant in nextVariants)
                    {
                        open.Add(frames.Append(frame).Append(new Frame(variant)).ToList());
                    }
                }

                return new Search(closed);
            }
        }
    }
}