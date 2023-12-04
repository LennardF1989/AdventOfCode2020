using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<TResult> SelectList<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector
        )
        {
            return source.Select(selector).ToList();
        }

        public static HashSet<TResult> SelectHashSet<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector
        )
        {
            return source.Select(selector).ToHashSet();
        }
    }
}
