using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day22
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day22Input.txt"));
            var sample = ConvertInput(File.ReadAllText("Inputs/Day22Sample.txt"));
            var timer = new MyTimer();

            PlayCombat(sample).Should().Be(306);
            PlayCombat(input).Should().Be(34005);

            timer.Lap();

            // Recursive termination check
            PlayRecursiveCombat(ConvertInput(@"Player 1:
43
19

Player 2:
2
29
14"), out var _);

            PlayRecursiveCombat(sample, out _).Should().Be(291);
            PlayRecursiveCombat(input, out _).Should().Be(32731);

            timer.Total();
        }

        internal static long PlayCombat(Day22Input initialState)
        {
            var result = Produce.Forever(initialState,
                state =>
                {
                    var z = state.Player1.Zip(state.Player2).ToList();
                    var nextState = z.Aggregate((player1: state.Player1.Skip(z.Count).ToList(), player2: state.Player2.Skip(z.Count).ToList()), (accumulator, current) =>
                        {
                            current.First.Should().NotBe(current.Second);
                            if (current.First > current.Second)
                            {
                                accumulator.player1.AddRange(new []{current.First, current.Second});
                            }
                            else
                            {
                                accumulator.player2.AddRange(new[] { current.Second, current.First });
                            }

                            return accumulator;
                        });
                    return new Day22Input(nextState.player1, nextState.player2);
                })
                .First(state => !state.Player1.Any() || !state.Player2.Any());
            var winner = result.Player1.Any() ? result.Player1 : result.Player2;
            return winner.Reverse().Select((it, index) => it * (index + 1)).Sum();
        }

        internal static long PlayRecursiveCombat(Day22Input state, out int winner)
        {
            var closed = new HashSet<Day22Input>();
            winner = 0;
            while (state.Player1.Any() && state.Player2.Any())
            {
                if (!closed.Add(state))
                {
                    winner = 0;
                    break;
                }

                var p1 = state.Player1[0];
                var p2 = state.Player2[0];
                if (state.Player1.Count - 1 >= p1 && state.Player2.Count - 1 >= p2)
                {
                    PlayRecursiveCombat(new Day22Input(state.Player1.Skip(1).Take(p1),
                        state.Player2.Skip(1).Take(p2)), out winner);
                }
                else
                {
                    winner = p1 > p2 ? 0 : 1;
                }
                state = winner == 0 
                    ? new Day22Input(state.Player1.Skip(1).Append(p1).Append(p2), state.Player2.Skip(1)) 
                    : new Day22Input(state.Player1.Skip(1), state.Player2.Skip(1).Append(p2).Append(p1));
            }
            var winnersHand = winner == 0 ? state.Player1 : state.Player2;
            return winnersHand.Reverse().Select((it, index) => it * (index + 1)).Sum();
        }

        private static Day22Input ConvertInput(string input)
        {
            var ps = input.SplitIntoParagraphs();
            return new Day22Input(ps[0].Skip(1).Select(it => Convert.ToInt32(it)),
                ps[1].Skip(1).Select(it => Convert.ToInt32(it)));
        }

        internal class Day22Input
        {
            public readonly IReadOnlyList<int> Player1;
            public readonly IReadOnlyList<int> Player2;
            private readonly int MyHashCode;

            public Day22Input(IEnumerable<int> player1, IEnumerable<int> player2)
            {
                Player1 = player1.ToList();
                Player2 = player2.ToList();
                MyHashCode = HashCode.Combine(Player1.Aggregate(0, (p, c) => HashCode.Combine(p, c)), Player2.Aggregate(0, (p, c) => HashCode.Combine(p, c)));
            }

            public override int GetHashCode()
            {
                return MyHashCode;
            }

            public override bool Equals(object? obj)
            {
                return obj is Day22Input other && Equals(other);
            }

            public bool Equals(Day22Input other)
            {
                return 
                    Player1.Count == other.Player1.Count &&
                    Player2.Count == other.Player2.Count &&
                    Player1.Zip(other.Player1).All(it => it.First == it.Second) && 
                    Player2.Zip(other.Player2).All(it => it.First == it.Second);
            }
        }
    }
}