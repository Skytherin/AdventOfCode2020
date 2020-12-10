using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public class Day08
    {
        private static readonly Instruction[] Input = ConvertInput(File.ReadAllText("Inputs/Day8.txt"));

        private static readonly Instruction[] Sample = ConvertInput(@"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6");

        public static void Run()
        {
            Part1();
            Part2ByBruteForce();
            Part2ByAnalysis();
        }

        public static void Part1()
        {
            Machine.Run(Sample).Accumulator.Should().Be(5);

            Machine.Run(Input).Accumulator.Should().Be(1709);
        }

        public static void Part2ByBruteForce()
        {
            var now = DateTime.Now;
            RepairByBruteForce(Sample.ToArray()).Should().Be(8);
            RepairByBruteForce(Input.ToArray()).Should().Be(1976);
            Console.WriteLine((DateTime.Now - now).TotalSeconds);
        }

        public static void Part2ByAnalysis()
        {
            var now = DateTime.Now;
            RepairByAnalysis(Sample).Should().Be(8);
            RepairByAnalysis(Input).Should().Be(1976);
            Console.WriteLine((DateTime.Now - now).TotalSeconds);
        }

        private static long RepairByAnalysis(Instruction[] instructions)
        {
            var markup = CreateMarkup(instructions);

            // Now walk forward, seeing if we can reach a "can reach end" node if we switch operations
            var current = markup[0];
            var seen = new HashSet<Markup>();
            while (current is InstructionMarkup c)
            {
                if (seen.Contains(current)) throw new ApplicationException();
                seen.Add(current);
                if (c.SwappedNext()?.CanReachEnd ?? false)
                {
                    var copy = instructions.ToArray();
                    copy[c.Index] = new Instruction
                    {
                        Argument = c.Instruction.Argument,
                        Operation = c.Instruction.Operation == "jmp" ? "nop" : "jmp"
                    };
                    return Machine.Run(copy).Accumulator;
                }

                current = current.Next();
            }
            throw new ApplicationException();
        }

        public static Markup[] CreateMarkup(Instruction[] instructions)
        {
            var markup = instructions.Select((instruction, index) => new InstructionMarkup(index, instruction) as Markup)
                .Append(new SentinelMarkup())
                .ToArray();

            // Set parents
            foreach (var item in markup.OfType<InstructionMarkup>())
            {
                item.SetNext(markup);
            }

            // Walk backwards, marking nodes that can reach the end
            var open = new Queue<Markup>();
            open.Enqueue(markup.Last());
            while (open.Any())
            {
                var current = open.Dequeue();
                current.CanReachEnd = true;
                open.EnqueueAll(current.Parents);
            }

            return markup;
        }

        public static long RepairByBruteForce(Instruction[] input)
        {
            for (var repairIndex = 0; repairIndex < input.Length; ++repairIndex)
            {
                var oldInstruction = input[repairIndex];
                if (input[repairIndex].Operation == "nop")
                {
                    input[repairIndex] = new Instruction {Argument = oldInstruction.Argument, Operation = "jmp"};
                }
                else if (input[repairIndex].Operation == "jmp")
                {
                    input[repairIndex] = new Instruction { Argument = oldInstruction.Argument, Operation = "nop" };
                }
                else continue;

                var vm = Machine.Run(input);
                if (vm.Termination == MachineTerminationType.Normal)
                {
                    return vm.Accumulator;
                }
                input[repairIndex] = oldInstruction;
            }
            throw new ApplicationException();
        }

        private static Instruction[] ConvertInput(string input)
        {
            return RegexUtils.DeserializeMany<Instruction>(input, @"(?<Operation>\w+) (?<Argument>[+-]\d+)")
                .ToArray();
        }
    }

    public class Instruction
    {
        public string Operation { get; set; } = "";
        public int Argument { get; set; }
    }

    public abstract class Markup
    {
        public List<InstructionMarkup> Parents { get; } = new List<InstructionMarkup>();
        
        public bool CanReachEnd { get; set; }

        public abstract Markup? Next();
    }

    public class SentinelMarkup: Markup
    {
        public SentinelMarkup()
        {
            CanReachEnd = true;
        }

        public override Markup? Next()
        {
            return null;
        }
    }

    public class InstructionMarkup: Markup
    {
        public readonly Instruction Instruction;
        public readonly int Index;
        protected Markup? LinearNext;
        protected Markup? JumpNext;

        public InstructionMarkup(int index, Instruction instruction)
        {
            Index = index;
            Instruction = instruction;
        }

        public void SetNext(Markup[] instructions)
        {
            var jumpTo = Index + Instruction.Argument;
            if (jumpTo.IsInRange(0, instructions.Length-1))
            {
                JumpNext = instructions[jumpTo];
            }

            var linearNext = Index + 1;
            if (linearNext.IsInRange(0, instructions.Length - 1))
            {
                LinearNext = instructions[linearNext];
            }

            Next()?.Parents.Add(this);
        }

        public override Markup? Next()
        {
            return Instruction.Operation switch
            {
                "jmp" => JumpNext,
                "nop" => LinearNext,
                _ => LinearNext
            };
        }

        public Markup? SwappedNext()
        {
            return Instruction.Operation switch
            {
                "jmp" => LinearNext,
                "nop" => JumpNext,
                _ => LinearNext
            };
        }
    }

    public enum MachineTerminationType
    {
        Normal,
        InfiniteLoopDetected
    }

    public class MachineResult
    {
        public MachineResult(long accumulator, MachineTerminationType termination)
        {
            Accumulator = accumulator;
            Termination = termination;
        }

        public MachineTerminationType Termination { get; }
        public long Accumulator{ get; }
    }

    public static class Machine
    {
        public static MachineResult Run(Instruction[] ram)
        {
            var pc = 0;
            var accumulator = 0L;

            var seen = new HashSet<int>();
            while (pc < ram.Length && !seen.Contains(pc))
            {
                seen.Add(pc);
                var instruction = ram[pc];
                switch (instruction.Operation)
                {
                    case "nop":
                        pc += 1;
                        break;
                    case "acc":
                        accumulator += instruction.Argument;
                        pc += 1;
                        break;
                    case "jmp":
                        pc += instruction.Argument;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return new MachineResult(accumulator, pc == ram.Length 
                ? MachineTerminationType.Normal 
                : MachineTerminationType.InfiniteLoopDetected);
        }
    }
}