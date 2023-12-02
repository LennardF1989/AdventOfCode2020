using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Extensions
{
    public static class ListExtensions
    {
        public static List<T> Overlaps<T>(this List<T> list, List<T> other)
        {
            return other.Where(list.Contains).ToList();
        }
    }
}
