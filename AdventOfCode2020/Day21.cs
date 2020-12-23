using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utils;
using FluentAssertions;

namespace AdventOfCode2020
{
    public static class Day21
    {
        public static void Run()
        {
            var input = ConvertInput(File.ReadAllText("Inputs/Day21Input.txt")).ToList();
            var sample = ConvertInput(File.ReadAllText("Inputs/Day21Sample.txt")).ToList();
            var timer = new MyTimer();

            Run1(sample, out var sampleCanonical).Should().Be(5);
            Run1(input, out var inputCanonical).Should().Be(2412);

            sampleCanonical.Should().Be("mxmxvkd,sqjhc,fvjkl");
            inputCanonical.Should().Be("mfp,mgvfmvp,nhdjth,hcdchl,dvkbjh,dcvrf,bcjz,mhnrqp");

            timer.Lap();


            timer.Total();
        }

        internal static long Run1(List<Day21Input> inputs, out string canonical)
        {
            var hashSets = inputs.SelectMany(input => input.Allergens.Select(allergen => (allergen, foods: input.Foods.ToList())))
                .GroupBy(it => it.allergen, it => it.foods)
                .ToDictionary(it => it.Key, it => it.Aggregate(it.First().ToHashSet(), (previous, current) => previous.Intersect(current).ToHashSet()));

            var done = false;
            while (!done)
            {
                done = true;
                foreach (var (k, food) in hashSets.Where(it => it.Value.Count == 1).Select(it => (it.Key, food: it.Value.First())).ToList())
                {
                    foreach (var (key, _) in hashSets.Where(it => it.Key != k).Where(it => it.Value.Contains(food)))
                    {
                        hashSets[key].Remove(food);
                        done = false;
                    }
                }
            }

            var allFoods = inputs.SelectMany(input => input.Foods).ToHashSet();

            var nonAllergenicFoods = allFoods.Where(food => hashSets.All(hs => !hs.Value.Contains(food)));

            canonical = allFoods.Where(food => !nonAllergenicFoods.Contains(food))
                .Select(food => (food, allergen: hashSets.Single(hs => hs.Value.Contains(food)).Key))
                .OrderBy(fta => fta.allergen)
                .Select(fta => fta.food)
                .Join(",");

            return inputs
                .Sum(input => input.Foods.Count(food => nonAllergenicFoods.Contains(food)));
        }

        private static IEnumerable<Day21Input> ConvertInput(string input)
        {
            foreach (var line in input.SplitIntoLines())
            {
                var m = RegexUtils.Deserialize(line, @"(?<foods>(\w+ )+)\s*\(contains (?<allergens>\w+(, \w+)*)\)",
                    new { foods = "", allergens = "" });

                yield return new Day21Input(m.foods.Trim().Split(" "), 
                    m.allergens.Trim().Split(",").Select(a => a.Trim()));
            }
        }

        internal class Day21Input
        {
            public readonly List<string> Foods;
            public readonly List<string> Allergens;

            public Day21Input(IEnumerable<string> foods, IEnumerable<string> allergens)
            {
                Foods = foods.ToList();
                Allergens = allergens.ToList();
            }
        }
    }
}