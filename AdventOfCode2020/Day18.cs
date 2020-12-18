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
            Run(ConvertInput("2 * 3 + (4 * 5)")).Should().Be(26);
            Run(ConvertInput("5 + (8 * 3 + 9 + 3 * 4 * 3)")).Should().Be(437);
            Run(ConvertInput("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))")).Should().Be(12240);
            Run(ConvertInput("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2")).Should().Be(13632);
            Run(input).Should().Be(464478013511L); 

            timer.Lap();

            Run2(ConvertInput("1 + (2 * 3) + (4 * (5 + 6))")).Should().Be(51);
            Run2(ConvertInput("2 * 3 + (4 * 5)")).Should().Be(46);
            Run2(ConvertInput("5 + (8 * 3 + 9 + 3 * 4 * 3)")).Should().Be(1445);
            Run2(ConvertInput("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))")).Should().Be(669060);
            Run2(ConvertInput("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2")).Should().Be(23340);
            Run2(input).Should().Be(85660197232452L);

            timer.Lap();
            timer.Total();
        }

        private static long Run(List<List<MathElement>> state)
        {
            return state.Sum(line => DoMath(new Queue<MathElement>(line), false));
        }

        private static long Run2(List<List<MathElement>> state)
        {
            return state.Sum(line => DoMath(new Queue<MathElement>(line), true));
        }

        private static long DoMath(Queue<MathElement> line, bool useOpPrecedence)
        {
            var first = ReadOneTerm(line, useOpPrecedence);
            while (line.Any())
            {
                var op = line.Dequeue();
                var second = ReadOneTerm(line, useOpPrecedence);
                if (op.Operator == '+')
                {
                    first = first + second;
                }
                else if (op.Operator == '*')
                {
                    first = first * second;
                }
                else throw new ApplicationException();
            }

            return first;
        }

        private static long ReadOneTerm(Queue<MathElement> line, bool useOpPrecedence)
        {
            var first = line.Dequeue();
            var result = 0L;
            if (first.Number is {} n) result = n;
            else if (first.Operator == '(')
            {
                var count = 1;
                var subTerm = new Queue<MathElement>();
                while (true)
                {
                    var next = line.Dequeue();
                    if (next.Operator == ')' && count == 1) break;
                    subTerm.Enqueue(next);
                    if (next.Operator == ')') count--;
                    if (next.Operator == '(') count++;
                }
                result = DoMath(subTerm, useOpPrecedence);
            }
            else throw new ApplicationException();

            if (useOpPrecedence && line.Count > 0 && line.Peek().Operator == '+')
            {
                line.Dequeue();
                var secondTerm = ReadOneTerm(line, useOpPrecedence);
                return result + secondTerm;
            }

            return result;
        }

        private static List<List<MathElement>> ConvertInput(string input)
        {
            return input.SplitIntoLines()
                .Select(line => RegexUtils.DeserializeMany<MathElement>(line, @"(?<Number>\d+)|(?<Operator>[+*()])"))
                .ToList();
        }
    }

    public class MathElement
    {
        public long? Number { get; set; }
        public char? Operator { get; set; }
    }
}