using System.Collections.Generic;

namespace AdventOfCode.Shared.Extensions
{
    public static class ListExtensions
    {
        public static List<T> Overlaps<T>(this List<T> list, List<T> other)
        {
            var overlap = new List<T>();

            foreach (T element in other)
            {
                if (list.Contains(element))
                {
                    overlap.Add(element);
                }
            }

            return overlap;
        }
    }
}
