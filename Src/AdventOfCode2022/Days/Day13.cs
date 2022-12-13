using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day13
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day13_Test.txt")
                .ReadAllLines("Content\\Day13.txt")
                ;

            var answer = 0;

            for (var i = 0; i < lines.Length / 3; i++)
            {
                var line1 = lines[i * 3];
                var line2 = lines[(i * 3) + 1];

                var r = CompareLine(line1, 0, line2, 0, out _, out _);

                if (r.HasValue && r.Value)
                {
                    answer += i + 1;
                }
            }

            Logger.Info($"Day 13A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day13_Test.txt")
                .ReadAllLines("Content\\Day13.txt")
                .Where(x => x != string.Empty)
                .ToList()
                ;

            var packet1 = "[[2]]";
            var packet2 = "[[6]]";

            lines.Add(packet1);
            lines.Add(packet2);

            var pairs = new Dictionary<string, int>();

            for (var i = 0; i < lines.Count; i++)
            {
                var line1 = lines[i];

                pairs.Add(line1, 0);

                for (var i2 = 0; i2 < lines.Count; i2++)
                {
                    if (i == i2)
                    {
                        continue;
                    }

                    var line2 = lines[i2];

                    var r = CompareLine(line1, 0, line2, 0, out _, out _);

                    if (r.HasValue && r.Value)
                    {
                        pairs[line1]++;
                    }
                }
            }

            var result = pairs
                .OrderByDescending(x => x.Value)
                .Select((x, i) => (line: x.Key, index: i))
                .ToList();

            var packet1Index = result.First(x => x.line == packet1).index + 1;
            var packet2Index = result.First(x => x.line == packet2).index + 1;

            var answer = packet1Index * packet2Index;

            Logger.Info($"Day 13B: {answer}");
        }

        private static bool? CompareLine(string line1, int startIndex1, string line2, int startIndex2, out int p1Length, out int p2Length)
        {
            var p1 = ReadPair(line1, startIndex1, out p1Length);
            var p2 = ReadPair(line2, startIndex2, out p2Length);

            var sp1 = p1.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var sp2 = p2.Split(",", StringSplitOptions.RemoveEmptyEntries);

            var spi1 = 0;
            var spi2 = 0;

            while (true)
            {
                if (spi1 >= sp1.Length && spi2 >= sp2.Length)
                {
                    break;
                }

                if (spi1 >= sp1.Length)
                {
                    //Left side ran out
                    return true;
                }

                if (spi2 >= sp2.Length)
                {
                    //Right side ran out
                    return false;
                }

                var s1 = sp1[spi1];
                var s2 = sp2[spi2];

                if (s1.StartsWith("[") && s2.StartsWith("["))
                {
                    var ns1 = string.Join(",", sp1.Skip(spi1));
                    var ns2 = string.Join(",", sp2.Skip(spi2));

                    var r = CompareLine(ns1, 0, ns2, 0, out var l1, out var l2);

                    sp1 = ns1.Substring(l1).Split(",", StringSplitOptions.RemoveEmptyEntries);
                    sp2 = ns2.Substring(l2).Split(",", StringSplitOptions.RemoveEmptyEntries);
                    spi1 = -1;
                    spi2 = -1;

                    if (r.HasValue)
                    {
                        return r.Value;
                    }
                }
                else if (s1.StartsWith("[") && !s2.StartsWith("["))
                {
                    var ns1 = string.Join(",", sp1.Skip(spi1));

                    var r = CompareLine(ns1, 0, $"[{s2}]", 0, out var l1, out var l2);

                    sp1 = ns1.Substring(l1).Split(",", StringSplitOptions.RemoveEmptyEntries);
                    spi1 = -1;

                    if (r.HasValue)
                    {
                        return r.Value;
                    }
                }
                else if (!s1.StartsWith("[") && s2.StartsWith("["))
                {
                    var ns2 = string.Join(",", sp2.Skip(spi2));

                    var r = CompareLine($"[{s1}]", 0, ns2, 0, out var l1, out var l2);

                    sp2 = ns2.Substring(l2).Split(",", StringSplitOptions.RemoveEmptyEntries);
                    spi2 = -1;

                    if (r.HasValue)
                    {
                        return r.Value;
                    }
                }
                else
                {
                    var n1 = int.Parse(s1);
                    var n2 = int.Parse(s2);

                    if (n1 < n2)
                    {
                        //Right order
                        return true;
                    }

                    if (n1 > n2)
                    {
                        //Not right order
                        return false;
                    }

                    //NOTE: Equal, continue...
                }

                spi1++;
                spi2++;
            }

            return null;
        }

        private static string ReadPair(string line, int startIndex, out int length)
        {
            var depth = 0;
            var result = new StringBuilder();

            for (var i = startIndex; i < line.Length; i++)
            {
                var c = line[i];

                result.Append(c);

                if (c == '[')
                {
                    depth++;
                }
                else if (c == ']')
                {
                    depth--;

                    if (depth == 0)
                    {
                        break;
                    }
                }
            }

            var result2 = result.ToString();

            length = result2.Length;

            return result2.Substring(1, result2.Length - 2);
        }
    }
}
