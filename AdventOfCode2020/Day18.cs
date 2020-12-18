using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day18
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day18.txt"));
            var timer = new MyTimer();
            var expression = new MathRdp();
            Run(ConvertInput("2 * 3 + (4 * 5)"), expression).Should().Be(26);
            Run(ConvertInput("5 + (8 * 3 + 9 + 3 * 4 * 3)"), expression).Should().Be(437);
            Run(ConvertInput("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))"), expression).Should().Be(12240);
            Run(ConvertInput("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2"), expression).Should().Be(13632);
            Run(input, expression).Should().Be(464478013511L); 

            timer.Lap();
            expression = new MathRdpWithPrecedence();
            Run(ConvertInput("1 + (2 * 3) + (4 * (5 + 6))"), expression).Should().Be(51);
            Run(ConvertInput("2 * 3 + (4 * 5)"), expression).Should().Be(46);
            Run(ConvertInput("5 + (8 * 3 + 9 + 3 * 4 * 3)"), expression).Should().Be(1445);
            Run(ConvertInput("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))"), expression).Should().Be(669060);
            Run(ConvertInput("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2"), expression).Should().Be(23340);
            Run(input, expression).Should().Be(85660197232452L);

            timer.Lap();
            timer.Total();
        }

        private static long Run(IEnumerable<List<MathToken>> state, MathRdp expression)
        {
            return state.Sum(line => expression.Expression(new Queue<MathToken>(line)));
        }

        private static List<List<MathToken>> ConvertInput(string input)
        {
            return input.SplitIntoLines()
                .Select(line => RegexUtils.DeserializeMany<MathToken>(line, @"(?<Number>\d+)|(?<Operator>[+*()])"))
                .ToList();
        }
    }

    public class MathToken
    {
        public long? Number { get; set; }
        public char? Operator { get; set; }
    }

    public class MathRdp
    {
        public long Expression(Queue<MathToken> line)
        {
            var first = Term(line);
            while (line.Any())
            {
                var op = line.Dequeue();
                var second = Term(line);
                first = op.Operator switch
                {
                    '+' => first + second,
                    '*' => first * second,
                    _ => throw new ApplicationException()
                };
            }

            return first;
        }

        protected virtual long Term(Queue<MathToken> line) => BaseTerm(line);

        protected long BaseTerm(Queue<MathToken> line)
        {
            var first = line.Dequeue();
            if (first.Number is { } n) return n;

            if (first.Operator == '(')
            {
                var count = 1;
                var subTerm = new Queue<MathToken>();

                while (true)
                {
                    var next = line.Dequeue();
                    if (next.Operator == ')' && count == 1) break;
                    subTerm.Enqueue(next);
                    if (next.Operator == ')') count--;
                    if (next.Operator == '(') count++;
                }
                return Expression(subTerm);
            }

            throw new ApplicationException();
        }
    }

    public class MathRdpWithPrecedence: MathRdp
    {
        protected override long Term(Queue<MathToken> line)
        {
            var result = BaseTerm(line);

            if (line.TryPeek(out var op) && op.Operator == '+')
            {
                line.Dequeue();
                var secondTerm = Term(line);
                return result + secondTerm;
            }

            return result;
        }
    }
}