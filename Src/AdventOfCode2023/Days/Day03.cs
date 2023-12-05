using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Extensions;

namespace AdventOfCode2023.Days
{
    public static class Day03
    {
        //NOTE: Original version - Using grid of nodes
        private static class Solution1
        {
            public enum EType
            {
                None = 0,
                Number = 1,
                Symbol = 2
            }

            public class Node
            {
                public (int, int) Position { get; set; }
                public char RawValue { get; set; }
                public int Value { get; set; }
                public EType Type { get; set; }
                public Node Left { get; set; }
                public Node Right { get; set; }
                public List<Node> Symbols { get; set; }
                public List<Node> Numbers { get; set; }

                public override string ToString()
                {
                    return $"[{Position.Item1}, {Position.Item2}] => {RawValue} ({Value})";
                }
            }

            private static readonly List<(int x, int y)> _surroundingCoords = new()
            {
                (-1, -1),
                (-1, 0),
                (-1, 1),
                (0, -1),
                (0, 0),
                (0, 1),
                (1, -1),
                (1, 0),
                (1, 1)
            };

            public static void StartA()
            {
                var lines = File
                    //.ReadAllLines("Content\\Day03_Test.txt")
                    .ReadAllLines("Content\\Day03.txt")
                    .SelectList(x => x.SelectList(y => y));

                var nodes = CreateNodes(lines);

                var maxY = lines.Count;
                var maxX = lines[0].Count;

                var answer = 0;

                for (var y = 0; y < maxY; y++)
                {
                    for (var x = 0; x < maxX; x++)
                    {
                        var currentNode = nodes[(x, y)];

                        if (currentNode.Type is EType.None or EType.Symbol)
                        {
                            continue;
                        }

                        var partNumber = 0;
                        var lastValidNode = currentNode;
                        var hasSymbol = false;

                        while (currentNode != null)
                        {
                            partNumber = (partNumber * 10) + currentNode.Value;

                            if (!hasSymbol)
                            {
                                hasSymbol = currentNode.Symbols.Any();
                            }

                            lastValidNode = currentNode;
                            currentNode = currentNode.Right;
                        }

                        if (hasSymbol)
                        {
                            answer += partNumber;
                        }

                        x = lastValidNode.Position.Item1;
                    }
                }

                Logger.Info($"Day 3A: {answer}");
            }

            public static void StartB()
            {
                var lines = File
                    //.ReadAllLines("Content\\Day03_Test.txt")
                    .ReadAllLines("Content\\Day03.txt")
                    .SelectList(x => x.SelectList(y => y));

                var nodes = CreateNodes(lines);

                var answer = 0;

                var gearNodes = nodes
                    .Where(x => x.Value.Type == EType.Symbol && x.Value.RawValue == '*')
                    .Select(x => x.Value.Numbers)
                    .Where(x => x.Count >= 2);

                foreach (var numberNodes in gearNodes)
                {
                    var visitedPositions = new HashSet<(int x, int y)>();
                    var partNumbers = new List<int>();

                    foreach (var nodeNumber in numberNodes)
                    {
                        if (visitedPositions.Contains(nodeNumber.Position))
                        {
                            continue;
                        }

                        //Scan to the left
                        var lastValidNode = nodeNumber;
                        var currentNode = nodeNumber;

                        while (currentNode != null)
                        {
                            lastValidNode = currentNode;
                            currentNode = currentNode.Left;
                        }

                        //Scan to the right
                        var partNumber = 0;
                        currentNode = lastValidNode;

                        while (currentNode != null)
                        {
                            visitedPositions.Add(currentNode.Position);

                            partNumber = (partNumber * 10) + currentNode.Value;

                            currentNode = currentNode.Right;
                        }

                        partNumbers.Add(partNumber);
                    }

                    if (partNumbers.Count >= 2)
                    {
                        answer += partNumbers.Aggregate(1, (x, y) => x * y);
                    }
                }

                Logger.Info($"Day 3B: {answer}");
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
                            Symbols = new List<Node>(),
                            Numbers = new List<Node>()
                        };

                        if (node.RawValue == '.')
                        {
                            node.Type = EType.None;
                        }
                        else if (char.IsNumber(node.RawValue))
                        {
                            node.Value = (int)char.GetNumericValue(node.RawValue);
                            node.Type = EType.Number;
                        }
                        else
                        {
                            node.Type = EType.Symbol;
                        }

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

                        //If this is a number, we want to link all numbers to the right.
                        if (currentNode.Type == EType.Number)
                        {
                            var newX = x + 1;
                            var newY = y;

                            if (newX >= 0 && newX < maxX && newY >= 0 && newY < maxY)
                            {
                                var otherNode = nodes[(newX, newY)];

                                if (otherNode.Type == EType.Number)
                                {
                                    otherNode.Left = currentNode;
                                    currentNode.Right = otherNode;
                                }
                            }
                        }

                        //If this is a number or a symbol, we want to get the neighboring nodes.
                        if (currentNode.Type is EType.Number or EType.Symbol)
                        {
                            _surroundingCoords.ForEach(tuple =>
                            {
                                var newX = x + tuple.x;
                                var newY = y + tuple.y;

                                if (newX < 0 || newX >= maxX || newY < 0 || newY >= maxY)
                                {
                                    return;
                                }

                                var otherNode = nodes[(newX, newY)];

                                if (otherNode.Type == EType.Symbol)
                                {
                                    currentNode.Symbols.Add(otherNode);
                                }
                                else if (otherNode.Type == EType.Number)
                                {
                                    currentNode.Numbers.Add(otherNode);
                                }
                            });
                        }
                    }
                }

                return nodes;
            }
        }

        //NOTE: Alternative version #1 - Using grid of nodes spanning over multiple coords (Box collision)
        private static class Solution2
        {
            private class Span
            {
                public int StartX { get; init; }
                public int StartY { get; init; }
                public int EndX { get; set; }
                public int EndY { get; set; }

                public bool Overlaps(Span span)
                {
                    return
                        StartY <= span.EndY && span.StartY <= EndY &&
                        StartX <= span.EndX && span.StartX <= EndX;
                }
            }

            private sealed class NumberSpan : Span
            {
                public int Value { get; set; }
            }

            private sealed class SymbolSpan : Span
            {
                public char Value { get; init; }
            }

            public static void StartA()
            {
                var spans = ParseInput();

                var numberSpans = spans
                    .Where(x => x is NumberSpan)
                    .Cast<NumberSpan>()
                    .ToList();

                var answer = spans
                    .Where(x => x is SymbolSpan)
                    .SelectMany(x => numberSpans.Where(x.Overlaps))
                    .Distinct()
                    .Sum(x => x.Value);

                Logger.Info($"Day 3A: {answer}");
            }

            public static void StartB()
            {
                var spans = ParseInput();

                var numberSpans = spans
                    .Where(x => x is NumberSpan)
                    .Cast<NumberSpan>()
                    .ToList();

                var answer = spans
                    .Where(x => x is SymbolSpan { Value: '*' })
                    .Select(x => numberSpans.Where(x.Overlaps).ToHashSet())
                    .Sum(x => x.Count >= 2 ? x.Aggregate(1, (y, z) => y * z.Value) : 0);

                Logger.Info($"Day 3B: {answer}");
            }

            private static List<Span> ParseInput()
            {
                var lines = File
                    //.ReadAllLines("Content\\Day03_Test.txt")
                    .ReadAllLines("Content\\Day03.txt")
                    .SelectList(x => x.SelectList(y => y));

                var maxY = lines.Count;
                var maxX = lines[0].Count;

                var spans = new List<Span>();

                for (var y = 0; y < maxY; y++)
                {
                    NumberSpan lastNumberSpan = null;

                    for (var x = 0; x < maxX; x++)
                    {
                        var c = lines[y][x];

                        if (c == '.')
                        {
                            lastNumberSpan = null;

                            continue;
                        }

                        if (char.IsNumber(c))
                        {
                            if (lastNumberSpan == null)
                            {
                                lastNumberSpan = new NumberSpan
                                {
                                    StartX = x,
                                    StartY = y
                                };

                                spans.Add(lastNumberSpan);
                            }

                            lastNumberSpan.Value = (lastNumberSpan.Value * 10) + (int)char.GetNumericValue(c);
                            lastNumberSpan.EndX = x;
                            lastNumberSpan.EndY = y;
                        }
                        else
                        {
                            lastNumberSpan = null;

                            spans.Add(new SymbolSpan
                            {
                                Value = c,
                                StartX = x - 1,
                                StartY = y - 1,
                                EndX = x + 1,
                                EndY = y + 1
                            });
                        }
                    }
                }

                return spans;
            }
        }

        //NOTE: Alternative version #2 - Using grid of nodes spanning over multiple coords (Moore neighborhood)
        private static class Solution3
        {
            private interface ISpan
            {
                //Do nothing
            }

            private sealed class NumberSpan : ISpan
            {
                public int Value { get; set; }
            }

            private sealed class SymbolSpan : ISpan
            {
                public char Value { get; init; }
            }

            private static readonly List<(int x, int y)> _surroundingCoords = new()
            {
                (-1, -1),
                (-1, 0),
                (-1, 1),
                (0, -1),
                (0, 1),
                (1, -1),
                (1, 0),
                (1, 1)
            };

            public static void StartA()
            {
                var spans = ParseInput();

                var answer = spans
                    .Where(x => x.Value is SymbolSpan)
                    .SelectMany(x => _surroundingCoords
                        .Select(y => spans.GetValueOrDefault((x.Key.x + y.x, x.Key.y + y.y)) as NumberSpan)
                        .WhereNotNull()
                    )
                    .Distinct()
                    .Sum(x => x.Value);

                Logger.Info($"Day 3A: {answer}");
            }

            public static void StartB()
            {
                var spans = ParseInput();

                var answer = spans
                    .Where(x => x.Value is SymbolSpan { Value: '*' })
                    .Select(x => _surroundingCoords
                        .Select(y => spans.GetValueOrDefault((x.Key.x + y.x, x.Key.y + y.y)) as NumberSpan)
                        .WhereNotNull()
                        .ToHashSet()
                    )
                    .Where(x => x.Count >= 2)
                    .Select(x => x.Aggregate(1, (y, z) => y * z.Value))
                    .Sum();

                Logger.Info($"Day 3B: {answer}");
            }

            private static Dictionary<(int x, int y), ISpan> ParseInput()
            {
                var lines = File
                    //.ReadAllLines("Content\\Day03_Test.txt")
                    .ReadAllLines("Content\\Day03.txt")
                    .SelectList(x => x.SelectList(y => y));

                var maxY = lines.Count;
                var maxX = lines[0].Count;

                var spans = new Dictionary<(int x, int y), ISpan>();

                for (var y = 0; y < maxY; y++)
                {
                    NumberSpan lastNumberSpan = null;

                    for (var x = 0; x < maxX; x++)
                    {
                        var c = lines[y][x];

                        if (c == '.')
                        {
                            lastNumberSpan = null;

                            continue;
                        }

                        if (char.IsNumber(c))
                        {
                            lastNumberSpan ??= new NumberSpan();

                            spans.Add((x, y), lastNumberSpan);

                            lastNumberSpan.Value = (lastNumberSpan.Value * 10) + (int)char.GetNumericValue(c);
                        }
                        else
                        {
                            lastNumberSpan = null;

                            spans.Add((x, y), new SymbolSpan
                            {
                                Value = c
                            });
                        }
                    }
                }

                return spans;
            }
        }

        public static void StartA()
        {
            //Solution1.StartA();
            //Solution2.StartA();
            Solution3.StartA();
        }

        public static void StartB()
        {
            //Solution1.StartB();
            //Solution2.StartB();
            Solution3.StartB();
        }
    }
}
