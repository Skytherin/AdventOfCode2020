using System;
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
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(result, value);
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(result, Convert.ToInt32(value));
                }
                else if (prop.PropertyType == typeof(char))
                {
                    prop.SetValue(result, value.First());
                }
                else
                {
                    throw new NotImplementedException("");
                }
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