using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Utils
{
    public static class RegexUtils
    {
        public static T Deserialize<T>(string input, string pattern) where T : new()
        {
            var match = Regex.Match(input, pattern);
            return MatchToObject<T>(match);
        }

        public static List<T> DeserializeMany<T>(string input, string pattern) where T: new()
        {
            var matches = Regex.Matches(input, pattern);
            return matches.Select(match => MatchToObject<T>(match)).ToList();
        }

        public static void TypesafeSet<T>(this PropertyInfo property, T result, string value)
        {
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(result, value);
            }
            else if (property.PropertyType == typeof(int?))
            {
                if (!string.IsNullOrWhiteSpace(value)) property.SetValue(result, Convert.ToInt32(value));
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValue(result, Convert.ToInt32(value));
            }
            else if (property.PropertyType == typeof(long?))
            {
                if (!string.IsNullOrWhiteSpace(value)) property.SetValue(result, Convert.ToInt64(value));
            }
            else if (property.PropertyType == typeof(long))
            {
                property.SetValue(result, Convert.ToInt64(value));
            }
            else if (property.PropertyType == typeof(char))
            {
                property.SetValue(result, value.First());
            }
            else
            {
                throw new NotImplementedException("");
            }
        }

        public static List<string> RxSplit(this string input, string pattern)
        {
            return Regex.Split(input, pattern).ToList();
        }

        public static List<string> SplitOnBlankLines(this string input)
        {
            return RxSplit(input, @"\s*\n\s*\n\s*");
        }

        public static List<string> SplitIntoLines(this string input)
        {
            return input.Split("\n").Select(it => it.Trim()).ToList();
        }

        private static T MatchToObject<T>(Match match) where T : new()
        {
            if (!match.Success) throw new ApplicationException();

            var type = typeof(T);
            var result = new T();
            var properties = type.GetProperties().Where(prop => prop.SetMethod != null).ToList();

            var requiredProperties = properties.Where(prop =>
            {
                var propertyType = prop.PropertyType;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return false;
                }

                if (prop.GetCustomAttribute<RxOptional>() != null)
                {
                    return false;
                }

                return true;
            }).ToList();

            foreach (var namedGroup in match.Groups.Keys)
            {
                var prop = properties.SingleOrDefault(p => p.Name == namedGroup);
                if (prop == null) continue;
                var value = match.Groups[namedGroup].Value;
                prop.TypesafeSet(result, value);
                requiredProperties.Remove(prop);
            }

            if (requiredProperties.Any())
            {
                throw new ApplicationException("Required properties missing");
            }

            return result;
        }
    }

    public class RxOptional: Attribute
    {

    }
}