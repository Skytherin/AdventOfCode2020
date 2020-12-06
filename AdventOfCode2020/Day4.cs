using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using FluentAssertions;
using AdventOfCode2020.Utils;

namespace AdventOfCode2020
{
    public class Day4
    {
        private static readonly List<Day4Input> SampleInput = @"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm

iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929

hcl:#ae17e1 iyr:2013
eyr:2024
ecl:brn pid:760753108 byr:1931
hgt:179cm

hcl:#cfa07d eyr:2025 pid:166559648
iyr:2011 ecl:brn hgt:59in".Replace("\r\n", "\n").Split("\n\n").Select(it => new Day4Input(it)).ToList();

        private static readonly List<Day4Input> Input = File.ReadAllText("Inputs/Day4.txt").Replace("\r\n", "\n").Split("\n\n").Select(it => new Day4Input(it)).ToList();

        public static void Run()
        {
            Part1().Should().BeEquivalentTo(2, 233);
            Part2().Should().BeEquivalentTo(111);
        }

        public static IEnumerable<long> Part1()
        {
            yield return RunAlgorithm1(SampleInput);
            yield return RunAlgorithm1(Input);
        }

        public static IEnumerable<long> Part2()
        {
            var invalidPassports = @"eyr:1972 cid:100
hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

iyr:2019
hcl:#602927 eyr:1967 hgt:170cm
ecl:grn pid:012533040 byr:1946

hcl:dab227 iyr:2012
ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

hgt:59cm ecl:zzz
eyr:2038 hcl:74454a iyr:2023
pid:3556412378 byr:2007".Replace("\r\n", "\n").Split("\n\n").Select(it => new Day4Input(it)).ToList();

            invalidPassports.Should().HaveCount(4);
            invalidPassports.Where(it => it.IsValidStrong()).Should().HaveCount(0);

            var validPassports = @"pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
hcl:#623a2f

eyr:2029 ecl:blu cid:129 byr:1989
iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

hcl:#888785
hgt:164cm byr:2001 iyr:2015 cid:88
pid:545766238 ecl:hzl
eyr:2022

iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719".Replace("\r\n", "\n").Split("\n\n").Select(it => new Day4Input(it)).ToList();

            validPassports.Should().HaveCount(4);
            validPassports.Where(it => it.IsValidStrong()).Should().HaveCount(4);

            yield return Input.Count(it => it.IsValidStrong());
        }


        private static long RunAlgorithm1(List<Day4Input> input)
        {
            return input.Count(it => it.IsValidWeak());
        }

    }

  
    public class Day4Input
    {
        public bool IsValidWeak()
        {
            return BirthYear != null
                   && IssueYear != null
                   && ExpirationYear != null
                   && Height != null
                   && EyeColor != null
                   && PassportId != null
                   && HairColor != null;
        }

        public bool IsValidStrong()
        {
            var context = new ValidationContext(this, null, null);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(this, context, validationResults, true);
            if (!result) return false;

            var height = RegexUtils.Deserialize<HeightValue>(Height ?? "", @"(?<Value>\d+)(?<Unit>.*)");
            return height.Unit switch
            {
                "cm" => height.Value.IsInRange(150, 193),
                "in" => height.Value.IsInRange(59, 76),
                _ => false
            };
        }

        [Key("byr")]
        [Required]
        [Range(1920, 2002)]
        public int? BirthYear { get; set; }

        [Key("iyr")]
        [Required]
        [Range(2010, 2020)]
        public int? IssueYear { get; set; }

        [Key("eyr")]
        [Required]
        [Range(2020, 2030)]
        public int? ExpirationYear { get; set; }

        [Key("hgt")]
        [Required]
        [RegularExpression(@"^\d+(cm|in)$")]
        public string? Height { get; set; }

        [Key("hcl")]
        [RegularExpression(@"^#[0-9a-f]{6}$")]
        [Required]
        public string? HairColor { get; set; }

        [Key("ecl"), RegularExpression("^amb|blu|brn|gry|grn|hzl|oth$")]
        [Required]
        public string? EyeColor { get; set; }

        [Key("pid"), RegularExpression("^[0-9]{9}$")]
        [Required]
        public string? PassportId { get; set; }

        public Day4Input(string input)
        {
            var data = input.Replace("\n", " ")
                .Split(" ")
                .Select(it => it.Trim())
                .Where(it => it != string.Empty)
                .Select(it =>
                {
                    var keyValue = it.Split(":");
                    return new {Key = keyValue[0], Value = keyValue[1]};
                })
                .ToDictionary(it => it.Key, it => it.Value);

            var props = GetType().Properties()
                .Select(it => new {P = it, K = it.GetCustomAttribute<KeyAttribute>()})
                .Where(it => it.K != null)
                .ToDictionary(it => it.K.Name, it => it.P);

            foreach (var (k, property) in props)
            {
                if (data.TryGetValue(k, out var value))
                {
                    property.TypesafeSet(this, value);
                }
            }
        }
    }

    public class KeyAttribute: Attribute
    {
        public readonly string Name;

        public KeyAttribute(string name)
        {
            Name = name;
        }
    }

    public class HeightValue
    {
        public int Value { set; get; }
        public string Unit { set; get; } = default!;
    }
}