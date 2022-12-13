using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day15
    {
        public class Node
        {
            public (int, int) Position { get; set; }
            public int Value { get; set; }

            public List<Node> Targets { get; set; }

            public override string ToString()
            {
                return $"[{Position.Item1}, {Position.Item2}] => {Value}";
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
                //.ReadAllLines("Content\\Day15_Test.txt")
                .ReadAllLines("Content\\Day15.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList();

            var nodes = CreateNodes(lines, out var width, out var height);

            /*var paths = new List<int>();
            DepthFirstSearchRecursive(nodes[(0, 0)], nodes[(width, height)], new List<Node>(), paths);
            int answer = paths.Min();*/

            /*var paths = new List<int>();
            DepthFirstSearchIterative(nodes[(0, 0)], nodes[(width, height)], paths);
            int answer = paths.Min();*/

            var path = AStar0(nodes[(0, 0)], nodes[(width, height)]);
            var answer = path.Skip(1).Sum(x => x.Value);

            Logger.Info($"Day 15A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day15_Test.txt")
                .ReadAllLines("Content\\Day15.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList();

            var grid = ResizeGrid(lines);
            var nodes = CreateNodes(grid, out var width, out var height);

            //var path = AStar0(nodes[(0, 0)], nodes[(width, height)]);
            //var answer = path.Skip(1).Sum(x => x.Value);
            
            //var answer = AStar1(nodes[(0, 0)], nodes[(width, height)]);
            
            var answer = AStar2(nodes[(0, 0)], nodes[(width, height)]);

            Logger.Info($"Day 15B: {answer}");
        }

        private static List<List<int>> ResizeGrid(List<List<int>> lines)
        {
            const int repeat = 5;

            var newGrid = new List<List<int>>();
            var newWidth = lines[0].Count * repeat;
            var newHeight = lines.Count * repeat;

            for (int y = 0; y < newHeight; y++)
            {
                var line = Enumerable.Repeat(0, newWidth).ToList();

                newGrid.Add(line);
            }

            //NOTE: Update top tiles horizontally
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[0].Count; x++)
                {
                    var value = lines[y][x];

                    newGrid[y][x] = value;

                    for (int i = 1; i < repeat; i++)
                    {
                        value++;

                        if (value > 9)
                        {
                            value = 1;
                        }

                        newGrid[y][x + (i * lines[0].Count)] = value;
                    }
                }
            }

            //NOTE: Update other tiles vertically
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    var value = newGrid[y][x];

                    newGrid[y][x] = value;

                    for (int i = 1; i < repeat; i++)
                    {
                        value++;

                        if (value > 9)
                        {
                            value = 1;
                        }

                        newGrid[y + (i * lines.Count)][x] = value;
                    }
                }
            }

            return newGrid;
        }

        //NOTE: Works for the test input, but not for the actual input.
        private static void DepthFirstSearchRecursive(Node start, Node target, List<Node> visited, List<int> paths)
        {
            visited.Add(start);

            if (paths.Count > 0)
            {
                var minPath = paths.Min();
                var currentPath = visited.Skip(1).Sum(x => x.Value);

                if (currentPath >= minPath)
                {
                    return;
                }
            }

            if (start == target)
            {
                paths.Add(visited.Skip(1).Sum(x => x.Value));
            }
            else
            {
                foreach (Node i in start.Targets)
                {
                    if (visited.Contains(i))
                    {
                        continue;
                    }

                    DepthFirstSearchRecursive(i, target, new List<Node>(visited), paths);
                }
            }
        }

        //NOTE: Works for the test input, but is just too slow for the actual input.
        private static void DepthFirstSearchIterative(Node start, Node target, List<int> paths)
        {
            Stack<(Node, List<Node>)> stack = new Stack<(Node, List<Node>)>();
            stack.Push((start, new List<Node>()));

            while (stack.Count > 0)
            {
                var entry = stack.Pop();
                var node = entry.Item1;
                var visited = entry.Item2;

                if (visited.Contains(node))
                {
                    continue;
                }

                if (paths.Count > 0)
                {
                    var minPath = paths.Min();
                    var currentPath = visited.Skip(1).Sum(x => x.Value);

                    if (currentPath >= minPath)
                    {
                        continue;
                    }
                }

                visited.Add(node);

                if (node == target)
                {
                    paths.Add(visited.Skip(1).Sum(x => x.Value));
                }
                else
                {
                    foreach (Node i in node.Targets)
                    {
                        if (visited.Contains(i))
                        {
                            continue;
                        }

                        stack.Push((i, new List<Node>(visited)));
                    }
                }
            }
        }

        //NOTE: Complete A*, bad performance.
        private static List<Node> AStar0(Node start, Node target)
        {
            List<(Node node, int cost)> openSet = new();
            Dictionary<Node, Node> parentMap = new();

            Dictionary<Node, int> gScore = new()
            {
                [start] = 0
            };

            Dictionary<Node, int> fScore = new()
            {
                [start] = 0
            };

            openSet.Add((start, fScore[start]));

            while(openSet.Count > 0)
            {
                var kvp = openSet.First(x => x.Item2 == openSet.Min(y => y.Item2));
                openSet.Remove(kvp);

                var node = kvp.node;

                if(node == target)
                {
                    return GetPathForNode(parentMap, node);
                }

                foreach(var neighbour in node.Targets)
                {
                    //g(n) = actual cost to reach n
                    //h(n) = estimated cost from n to target
                    //f(n) = g(n) + h(n)
                    var cost = gScore[node] + neighbour.Value;

                    if (gScore.ContainsKey(neighbour) && cost >= gScore[neighbour])
                    {
                        continue;
                    }

                    parentMap[neighbour] = node;
                    gScore[neighbour] = cost;

                    fScore[neighbour] = cost + ManhattanDistance(neighbour, target);

                    openSet.Add((neighbour, fScore[neighbour]));
                }
            }

            return null;
        }

        //NOTE: Trimmed A*, still bad performance.
        private static int AStar1(Node start, Node target)
        {
            List<(Node node, int cost)> openSet = new();

            Dictionary<Node, int> gScore = new()
            {
                [start] = 0
            };

            openSet.Add((start, 0));

            while(openSet.Count > 0)
            {
                var kvp = openSet.First(x => x.Item2 == openSet.Min(y => y.Item2));
                openSet.Remove(kvp);

                var node = kvp.node;

                if(node == target)
                {
                    return gScore[target];
                }

                foreach(var neighbour in node.Targets)
                {
                    var cost = gScore[node] + neighbour.Value;

                    if (gScore.ContainsKey(neighbour) && cost >= gScore[neighbour])
                    {
                        continue;
                    }

                    gScore[neighbour] = cost;

                    openSet.Add((neighbour, cost + ManhattanDistance(neighbour, target)));
                }
            }

            return -1;
        }

        //NOTE: Trimmed A*, best performance due to PriorityQueue.
        private static int AStar2(Node start, Node target)
        {
            var openSet = new PriorityQueue<Node, int>();

            var gScore = new Dictionary<Node, int>
            {
                [start] = 0
            };

            openSet.Enqueue(start, 0);

            while (openSet.Count > 0)
            {
                var node = openSet.Dequeue();

                if (node == target)
                {
                    return gScore[target];
                }

                foreach (var neighbour in node.Targets)
                {
                    var cost = gScore[node] + neighbour.Value;

                    if (gScore.ContainsKey(neighbour) && cost >= gScore[neighbour])
                    {
                        continue;
                    }
                    
                    gScore[neighbour] = cost;

                    openSet.Enqueue(neighbour, cost + ManhattanDistance(node, target));
                }
            }

            return -1;
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

            while(parentMap.ContainsKey(current))
            {
                current = parentMap[current];

                path.Add(current);
            }

            path.Reverse();

            return path;
        }

        private static Dictionary<(int x, int y), Node> CreateNodes(List<List<int>> lines, out int width, out int height)
        {
            var nodes = lines
                .SelectMany((y, i) => y.Select((x, i2) => new Node
                {
                    Position = (i, i2),
                    Value = x,
                    Targets = new List<Node>()
                }))
                .ToDictionary(x => x.Position, x => x);

            int maxX = lines[0].Count;
            int maxY = lines.Count;

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    _surroundingCoords.ForEach(tuple =>
                    {
                        int newX = x + tuple.x;
                        int newY = y + tuple.y;

                        int value = GetValue(lines, newX, newY);

                        if (value > -1)
                        {
                            nodes[(x, y)].Targets.Add(nodes[(newX, newY)]);
                        }
                    });
                }
            }

            width = maxX - 1;
            height = maxY - 1;

            return nodes;
        }

        private static int GetValue(List<List<int>> lines, int x, int y)
        {
            if (y < 0 || y >= lines.Count || x < 0 || x >= lines[y].Count)
            {
                return -1;
            }

            return lines[y][x];
        }
    }
}
