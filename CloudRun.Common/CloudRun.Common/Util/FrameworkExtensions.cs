using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudRun.Common.Util
{
    public static class FrameworkExtensions
    {
        public static bool IsEither(this string str, params string[] values)
        {
            return IsEither(str, values.AsEnumerable());
        }

        public static bool IsEither(this string str, IEnumerable<string> values)
        {
            if (str == null && values == null)
                return true;

            bool result = false;

            foreach (string val in values)
            {
                if (str.Eq(val))
                {
                    result = true;

                    break;  // BREAK !!!
                }
            }

            return result;
        }

        public static bool IsNeither(this string str, params string[] values)
        {
            return !IsEither(str, values.AsEnumerable());
        }

        public static bool IsNeither(this string str, IEnumerable<string> values)
        {
            return !IsEither(str, values);
        }

        public static bool IsEitherOrEmpty(this string str, IEnumerable<string> values)
        {
            if (string.IsNullOrEmpty(str))
                return true;

            return IsEither(str, values);
        }

        public static bool Eq(this string str, string otherStr)
        {
            if (str == null)
            {
                return otherStr == null;
            }
            else
            {
                return str.Equals(otherStr, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool Has(this string str, string otherStr)
        {
            if (str == null || otherStr == null)
                return false;

            return str.IndexOf(otherStr, StringComparison.OrdinalIgnoreCase) > -1;
        }

        public static bool HasAny(this string str, params string[] otherStrArr)
        {
            if (str == null || otherStrArr == null || otherStrArr.Length == 0)
                return false;

            foreach (var otherStr in otherStrArr)
            {
                var has = Has(str, otherStr);

                if (has)
                {
                    return has;
                }
            }

            return false;
        }

        public static bool Empty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
