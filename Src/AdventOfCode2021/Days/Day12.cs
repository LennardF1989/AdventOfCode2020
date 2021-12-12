using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day12
    {
        public class Node
        {
            public string Name { get; set; }

            public bool IsSmallCave => char.IsLower(Name[0]);

            public List<Node> Targets { get; set; } = new();

            public override string ToString()
            {
                return Name;
            }
        }

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day12_Test.txt")
                .ReadAllLines("Content\\Day12.txt")
                .ToList();

            var nodes = GetNodes(lines);

            int answer = CountPathsFromTo(
                nodes["start"],
                nodes["end"],
                0,
                new List<Node>()
            );

            Logger.Info($"Day 12A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day12_Test.txt")
                .ReadAllLines("Content\\Day12.txt")
                .ToList();

            var nodes = GetNodes(lines);

            int answer = CountPathsFromTo2(
                nodes["start"],
                nodes["end"],
                0,
                new List<Node>()
            );

            Logger.Info($"Day 12B: {answer}");
        }

        private static Dictionary<string, Node> GetNodes(List<string> lines)
        {
            var nodes = lines
                .SelectMany(x => x.Split("-"))
                .Distinct()
                .Select(x => new Node
                {
                    Name = x
                })
                .ToDictionary(x => x.Name);

            lines.ForEach(s =>
            {
                var leftRight = s.Split("-");

                nodes[leftRight[0]].Targets.Add(nodes[leftRight[1]]);
                nodes[leftRight[1]].Targets.Add(nodes[leftRight[0]]);
            });

            return nodes;
        }

        private static int CountPathsFromTo(Node current, Node target, int pathCount, List<Node> visited)
        {
            visited.Add(current);

            if (current == target)
            {
                pathCount++;

                //Logger.Debug(string.Join(",", visited.Select(x => x.Name)));
            }
            else
            {
                foreach (Node i in current.Targets)
                {
                    if (i.IsSmallCave && visited.Contains(i))
                    {
                        continue;
                    }

                    pathCount = CountPathsFromTo(i, target, pathCount, new List<Node>(visited));
                }
            }

            return pathCount;
        }

        private static int CountPathsFromTo2(Node current, Node target, int pathCount, List<Node> visited)
        {
            visited.Add(current);

            var smallCaveVisitedTwice = visited
                .Where(x => x.Name != "start" && x.Name != "end" && x.IsSmallCave)
                .GroupBy(x => x.Name)
                .FirstOrDefault(x => x.Count() == 2)
                ?.FirstOrDefault();

            if (current == target)
            {
                pathCount++;

                //Logger.Debug(string.Join(",", visited.Select(x => x.Name)));
            }
            else
            {
                foreach (Node i in current.Targets)
                {
                    if (i.Name == "start" ||
                        (smallCaveVisitedTwice != null && i.IsSmallCave && visited.Contains(i)))
                    {
                        continue;
                    }

                    pathCount = CountPathsFromTo2(i, target, pathCount, new List<Node>(visited));
                }
            }

            return pathCount;
        }
    }
}
