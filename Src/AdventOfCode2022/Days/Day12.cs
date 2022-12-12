using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day12
    {
        public class Node
        {
            public (int, int) Position { get; set; }
            public char RawValue { get; set; }
            public int Value { get; set; }
            public List<Node> Targets { get; set; }

            public override string ToString()
            {
                return $"[{Position.Item1}, {Position.Item2}] => {RawValue} ({Value})";
            }
        }

        //NOTE: Von Neumann neighborhood
        private static readonly List<(int x, int y)> _surroundingCoords = new()
        {
            (0, -1),
            (-1, 0),
            (1, 0),
            (0, 1)
        };

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day12_Test.txt")
                .ReadAllLines("Content\\Day12.txt")
                .Select(x => new List<char>(x.ToCharArray()))
                .ToList()
                ;

            var nodes = CreateNodes(lines);
            var startNode = nodes.First(x => x.Value.RawValue == 'S').Value;
            var endNode = nodes.First(x => x.Value.RawValue == 'E').Value;
            var pathNodes = AStar(startNode, endNode);

            var answer = pathNodes.Count - 1;
            
            Logger.Info($"Day 12A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                    //.ReadAllLines("Content\\Day12_Test.txt")
                    .ReadAllLines("Content\\Day12.txt")
                    .Select(x => new List<char>(x.ToCharArray()))
                    .ToList()
                ;

            var nodes = CreateNodes(lines);
            var startNodes = nodes
                .Where(x => x.Value.RawValue == 'S' || x.Value.RawValue == 'a')
                .Select(x => x.Value)
                .ToList();
            var endNode = nodes.First(x => x.Value.RawValue == 'E').Value;

            var lowest = int.MaxValue;

            foreach (var startNode in startNodes)
            {
                var pathNodes = AStar(startNode, endNode);

                if (pathNodes != null && pathNodes.Count > 1 && pathNodes.Count - 1 < lowest)
                {
                    lowest = pathNodes.Count - 1;
                }
            }

            var answer = lowest;

            Logger.Info($"Day 12B: {answer}");
        }

        //Based on: AStar0 and AStar2 from Day15 of AoC2021
        private static List<Node> AStar(Node start, Node target)
        {
            var openSet = new PriorityQueue<Node, int>();
            var parentMap = new Dictionary<Node, Node>();

            var gScore = new Dictionary<Node, int>
            {
                [start] = 0
            };

            var closedSet = new HashSet<Node>();

            openSet.Enqueue(start, 0);

            while (openSet.Count > 0)
            {
                var node = openSet.Dequeue();

                if (!closedSet.Add(node))
                {
                    continue;
                }

                if (node == target)
                {
                    return GetPathForNode(parentMap, target);
                }

                foreach (var neighbor in node.Targets)
                {
                    //BUGFIX: The cost of moving is always 1
                    var cost = gScore[node] + 1;

                    if (cost >= gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    {
                        continue;
                    }

                    parentMap[neighbor] = node;
                    gScore[neighbor] = cost;

                    openSet.Enqueue(neighbor, cost + ManhattanDistance(neighbor, target));
                }
            }

            return null;
        }

        private static int ManhattanDistance(Node a, Node b)
        {
            var x = Math.Abs(a.Position.Item1 - b.Position.Item1);
            var y = Math.Abs(a.Position.Item2 - b.Position.Item2);

            return x + y;
        }

        private static List<Node> GetPathForNode(Dictionary<Node, Node> parentMap, Node current)
        {
            List<Node> path = new()
            {
                current
            };

            while (parentMap.TryGetValue(current, out var next))
            {
                path.Add(next);
                current = next;
            }

            path.Reverse();

            return path;
        }

        private static Dictionary<(int x, int y), Node> CreateNodes(List<List<char>> lines)
        {
            var nodes = lines
                .SelectMany((y, i) => y.Select((x, i2) =>
                {
                    var node = new Node
                    {
                        Position = (i2, i),
                        RawValue = x,
                        Targets = new List<Node>()
                    };

                    node.Value = node.RawValue switch
                    {
                        'S' => 1,
                        'E' => 26,
                        _ => node.RawValue - 96
                    };

                    return node;
                }))
                .ToDictionary(x => x.Position);

            var maxY = lines.Count;
            var maxX = lines[0].Count;

            for (var y = 0; y < maxY; y++)
            {
                for (var x = 0; x < maxX; x++)
                {
                    var currentNode = nodes[(x, y)];

                    _surroundingCoords.ForEach(tuple =>
                    {
                        var newX = x + tuple.x;
                        var newY = y + tuple.y;

                        if (newX < 0 || newX >= maxX || newY < 0 || newY >= maxY)
                        {
                            return;
                        }

                        var otherNode = nodes[(newX, newY)];

                        //BUGFIX: Anything lower is also allowed...
                        if (otherNode.Value <= currentNode.Value + 1)
                        {
                            currentNode.Targets.Add(otherNode);
                        }
                    });
                }
            }

            return nodes;
        }
    }
}