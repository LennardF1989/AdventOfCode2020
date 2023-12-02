using System;

namespace AdventOfCode.Shared.Extensions
{
    public static class StringExtensions
    {
        public static int ToInteger(this string str)
        {
            return int.Parse(str);
        }

        public static string[] Split(this string str, string separator, bool trimEntries = false, bool removeEmptyEntries = false)
        {
            var options = StringSplitOptions.None;

            if (trimEntries)
            {
                options |= StringSplitOptions.TrimEntries;
            }

            if (removeEmptyEntries)
            {
                options |= StringSplitOptions.RemoveEmptyEntries;
            }

            return str.Split(separator, options);
        }
    }
}
