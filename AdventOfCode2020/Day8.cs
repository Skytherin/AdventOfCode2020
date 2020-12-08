using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public class Day8
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
            Part2();
        }

        public static void Part1()
        {
            new VirtualMachine(Sample).Run().Should().Be(5);

            new VirtualMachine(Input).Run().Should().Be(1709);
        }

        public static void Part2()
        {
            Repair(Sample).Should().Be(8);
            Repair(Input).Should().Be(1976);
        }

        public static long Repair(Instruction[] input)
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
                var vm = new VirtualMachine(input);
                vm.Run();
                if (vm.TerminatedNormally)
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

    public class VirtualMachine
    {
        private readonly Instruction[] RAM;
        private int PC = 0;
        public long Accumulator = 0;
        public bool TerminatedNormally = false;

        public VirtualMachine(Instruction[] ram)
        {
            RAM = ram;
        }

        public long Run()
        {
            var seen = new HashSet<int>();
            while (PC < RAM.Length && !seen.Contains(PC))
            {
                seen.Add(PC);
                var instruction = RAM[PC];
                switch (instruction.Operation)
                {
                    case "nop":
                        PC += 1;
                        break;
                    case "acc":
                        Accumulator += instruction.Argument;
                        PC += 1;
                        break;
                    case "jmp":
                        PC += instruction.Argument;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (PC == RAM.Length) TerminatedNormally = true;

            return Accumulator;
        }
    }
}