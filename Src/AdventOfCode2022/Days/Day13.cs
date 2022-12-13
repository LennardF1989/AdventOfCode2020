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
        //NOTE: Original version
        static class Solution1
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

                    var result = CompareLine(line1, 0, line2, 0, out _, out _);

                    //Logger.Debug($"Pair {i + 1}: {result}");

                    if (result.HasValue && result.Value)
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

                const string packet1 = "[[2]]";
                const string packet2 = "[[6]]";

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

                var sortedPairs = pairs
                    .OrderByDescending(x => x.Value)
                    .Select((x, i) => (line: x.Key, index: i))
                    .ToList();

                var packet1Index = sortedPairs.First(x => x.line == packet1).index + 1;
                var packet2Index = sortedPairs.First(x => x.line == packet2).index + 1;

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

                        sp1 = ns1[l1..].Split(",", StringSplitOptions.RemoveEmptyEntries);
                        sp2 = ns2[l2..].Split(",", StringSplitOptions.RemoveEmptyEntries);
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

                        var r = CompareLine(ns1, 0, $"[{s2}]", 0, out var l1, out _);

                        sp1 = ns1[l1..].Split(",", StringSplitOptions.RemoveEmptyEntries);
                        spi1 = -1;

                        if (r.HasValue)
                        {
                            return r.Value;
                        }
                    }
                    else if (!s1.StartsWith("[") && s2.StartsWith("["))
                    {
                        var ns2 = string.Join(",", sp2.Skip(spi2));

                        var r = CompareLine($"[{s1}]", 0, ns2, 0, out _, out var l2);

                        sp2 = ns2[l2..].Split(",", StringSplitOptions.RemoveEmptyEntries);
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

        //NOTE: Optimized version
        static class Solution2
        {
            class Node
            {
                //Do nothing
            }

            class Array : Node
            {
                public List<Node> Nodes { get; }

                public Array()
                {
                    Nodes = new List<Node>();
                }

                public override string ToString()
                {
                    return $"[{string.Join(",", Nodes)}]";
                }
            }

            class Literal : Node
            {
                public int Value { get; init; }

                public override string ToString()
                {
                    return Value.ToString();
                }
            }

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

                    var tree1 = BuildTree(line1);
                    var tree2 = BuildTree(line2);

                    var result = CompareTree(tree1, tree2);

                    //Logger.Debug($"Pair {i+1}: {result}");

                    if (result.HasValue && result.Value)
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

                const string packet1 = "[[2]]";
                const string packet2 = "[[6]]";

                lines.Add(packet1);
                lines.Add(packet2);

                var trees = lines.Select(BuildTree).ToList();
                var packet1Tree = trees[^2];
                var packet2Tree = trees[^1];

                var pairs = new Dictionary<Array, int>();

                for (var i = 0; i < trees.Count; i++)
                {
                    var tree1 = trees[i];

                    pairs.Add(tree1, 0);

                    for (var i2 = 0; i2 < trees.Count; i2++)
                    {
                        if (i == i2)
                        {
                            continue;
                        }

                        var tree2 = trees[i2];

                        var result = CompareTree(tree1, tree2);

                        if (result.HasValue && result.Value)
                        {
                            pairs[tree1]++;
                        }
                    }
                }

                var sortedPairs = pairs
                    .OrderByDescending(x => x.Value)
                    .Select((x, i) => (line: x.Key, index: i))
                    .ToList();

                var packet1Index = sortedPairs.First(x => x.line == packet1Tree).index + 1;
                var packet2Index = sortedPairs.First(x => x.line == packet2Tree).index + 1;

                var answer = packet1Index * packet2Index;

                Logger.Info($"Day 13B: {answer}");
            }

            private static Array BuildTree(string line)
            {
                var root = new Array();

                var stack = new Stack<Array>();
                stack.Push(root);

                for (var i = 1; i < line.Length - 1; i++)
                {
                    var c = line[i];

                    if (c == ',')
                    {
                        continue;
                    }

                    if (c == '[')
                    {
                        var node = new Array();

                        stack.Peek().Nodes.Add(node);
                        stack.Push(node);
                    }
                    else if (c == ']')
                    {
                        stack.Pop();
                    }
                    else
                    {
                        var numberString = string.Empty;

                        for (; i < line.Length; i++)
                        {
                            var n = line[i];

                            if (!char.IsNumber(n))
                            {
                                i--;

                                break;
                            }

                            numberString += n;
                        }

                        var node = new Literal
                        {
                            Value = int.Parse(numberString)
                        };

                        stack.Peek().Nodes.Add(node);
                    }
                }

                return root;
            }

            private static bool? CompareTree(Array tree1, Array tree2)
            {
                var sp1 = tree1.Nodes;
                var sp2 = tree2.Nodes;

                var spi1 = 0;
                var spi2 = 0;

                while (true)
                {
                    if (spi1 >= sp1.Count && spi2 >= sp2.Count)
                    {
                        break;
                    }

                    if (spi1 >= sp1.Count)
                    {
                        //Left side ran out
                        return true;
                    }

                    if (spi2 >= sp2.Count)
                    {
                        //Right side ran out
                        return false;
                    }

                    var s1 = sp1[spi1];
                    var s2 = sp2[spi2];

                    if (s1 is Array && s2 is Array)
                    {
                        var r = CompareTree(s1 as Array, s2 as Array);

                        if (r.HasValue)
                        {
                            return r.Value;
                        }
                    }
                    else if (s1 is Array && s2 is Literal)
                    {
                        var r = CompareTree(s1 as Array, new Array { Nodes = { s2 } });

                        if (r.HasValue)
                        {
                            return r.Value;
                        }
                    }
                    else if (s1 is Literal && s2 is Array)
                    {
                        var r = CompareTree(new Array { Nodes = { s1 } }, s2 as Array);

                        if (r.HasValue)
                        {
                            return r.Value;
                        }
                    }
                    else
                    {
                        var n1 = ((Literal)s1).Value;
                        var n2 = ((Literal)s2).Value;

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
        }

        public static void StartA()
        {
            //Solution1.StartA();
            Solution2.StartA();
        }

        public static void StartB()
        {
            //Solution1.StartB();
            Solution2.StartB();
        }
    }
}
