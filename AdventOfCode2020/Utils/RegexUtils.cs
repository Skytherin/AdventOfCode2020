using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Utils
{
    public static class RegexUtils
    {
        public static T Deserialize<T>(string input, string pattern)
        {
            var match = Regex.Match(input, pattern);
            return MatchToObject<T>(match);
        }

        public static T Deserialize<T>(string input, string pattern, T template)
        {
            var match = Regex.Match(input, pattern);
            return MatchToObject<T>(match);
        }

        public static List<T> DeserializeMany<T>(string input, string pattern)
        {
            var matches = Regex.Matches(input, pattern);
            return matches.Select(match => MatchToObject<T>(match)).ToList();
        }

        public static void TypesafeSet<T>(this PropertyInfo prop, T instance, string value)
        {
            TypesafeSet(prop.PropertyType, value, write => prop.SetValue(instance, write));
        }

        public static void TypesafeSet<T>(this FieldInfo field, T instance, string value)
        {
            TypesafeSet(field.FieldType, value, write => field.SetValue(instance, write));
        }

        public static void TypesafeSet(Type type, string value, Action<object> setValue)
        {
            if (TryConvert(type, value, out var actual))
            {
                setValue(actual);
            }
        }

        public static bool TryConvert(Type type, string value, out object actual)
        {
            actual = value;

            if (type == typeof(string))
            {
                return true;
            }
            if (type == typeof(int?))
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    actual = Convert.ToInt32(value);
                    return true;
                }

                return false;
            }
            if (type == typeof(int))
            {
                actual = Convert.ToInt32(value);
                return true;
            }
            if (type == typeof(long?))
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    actual = Convert.ToInt64(value);
                    return true;
                }

                return false;
            }
            if (type == typeof(long))
            {
                actual = Convert.ToInt64(value);
                return true;
            }
            if (type == typeof(char))
            {
                actual = value.First();
                return true;
            }
            if (type == typeof(char?))
            {
                if (value.Length > 0)
                {
                    actual = value.First();
                    return true;
                }

                return false;
            }
            throw new NotImplementedException("");
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

        private static T MatchToObject<T>(Match match)
        {
            if (!match.Success) throw new ApplicationException();

            var type = typeof(T);

            var ctors = type.GetConstructors();
            var noParameterCtor = ctors.SingleOrDefault(ctor => ctor.GetParameters().Length == 0);
            if (noParameterCtor != null)
            {
                return MatchObjectFromProperties<T>(noParameterCtor.Invoke(new object[] { }), match);
            }

            var ctor = ctors.First();

            var parameters = ctor.GetParameters();
            var actuals = new List<object>();

            foreach (var parameter in parameters)
            {
                var value = match.Groups[parameter.Name ?? throw new ApplicationException()].Value;
                if (TryConvert(parameter.ParameterType, value, out var actual))
                {
                    actuals.Add(actual);
                }
                else
                {
                    throw new ApplicationException();
                }
            }

            return (T)ctor.Invoke(actuals.ToArray());
        }

        private static T MatchObjectFromProperties<T>(object instance, Match match) 
        {
            var type = typeof(T);
            var members = new List<MemberInfo>();
            members.AddRange(type.GetProperties().Where(prop => prop.SetMethod != null));
            members.AddRange(type.GetFields().Where(field => !field.IsInitOnly));
            if (!members.Any())
            {
                throw new ApplicationException();
            }

            var requiredProperties = members.Where(prop =>
            {
                var propertyType = prop.DeclaringType ?? throw new ApplicationException();
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return false;
                }

                return true;
            }).ToList();

            foreach (var namedGroup in match.Groups.Keys)
            {
                var member = members.SingleOrDefault(p => p.Name == namedGroup);
                if (member == null) continue;
                var value = match.Groups[namedGroup].Value;
                switch (member)
                {
                    case PropertyInfo prop:
                        prop.TypesafeSet(instance, value);
                        break;
                    case FieldInfo field:
                        field.TypesafeSet(instance, value);
                        break;
                    default:
                        throw new ApplicationException();
                }
                requiredProperties.Remove(member);
            }

            if (requiredProperties.Any())
            {
                throw new ApplicationException("Required properties missing");
            }

            return (T)instance;
        }
    }
}