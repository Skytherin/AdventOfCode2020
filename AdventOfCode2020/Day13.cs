using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day13
    {
        public static void Run()
        {
            var startTime = 1000066L;
            var x = -1;
            var input = new long [] { 13, x, x, 41, x, x, x, 37, x, x, x, x, x, 659, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, 19, x, x, x, 23, x, x, x, x, x, 29, x, 409, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, 17 };
            
            Part1(startTime, input).Should().Be(246);

            Part2(7, 13, x, x, 59, x, 31, 19).Should().Be(1068781L);
            Part2(17, x, 13, 19).Should().Be(3417);
            Part2(67, 7, 59, 61).Should().Be(754018);
            Part2(67, x, 7, 59, 61).Should().Be(779210);
            Part2(67, 7, x, 59, 61).Should().Be(1261476);
            Part2(1789, 37, 47, 1889).Should().Be(1202161486);

            Part2(input).Should().Be(939490236001473L);
        }

        private static long Part1(long startTime, IEnumerable<long> busIds)
        {
            return busIds
                .Where(id => id != -1)
                .Select(id => new { eta = id - (startTime % id), id })
                .OrderBy(it => it.eta)
                .Select(it => it.eta * it.id)
                .First();
        }

        private static long Part2(params long[] buses)
        {
            return buses
                .Select((id, index) => new { id, index })
                .Where(bus => bus.id != -1)
                .Skip(1)
                .Aggregate((value: buses[0], increment: buses[0]), (state, bus) => (
                    value: Produce.Forever(state.value, current => current + state.increment)
                        .First(current => (current + bus.index).IsMultipleOf(bus.id)),
                    increment: state.increment * bus.id
                ))
                .value;
        }
    }
}