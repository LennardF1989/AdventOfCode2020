using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day14
    {
        enum State
        {
            Air = 0,
            Rock,
            Sand
        }

        public static void StartA()
        {
            var lines = File
                    //.ReadAllLines("Content\\Day14_Test.txt")
                    .ReadAllLines("Content\\Day14.txt")
                    .Select(ParseInput)
                    .ToList()
                    ;

            var(minX, maxX, minY, maxY) = DetermineGridDimensions(lines);

            //NOTE: Always 0
            minY = 0;

            var grid = GenerateGrid(lines, minX, maxX, minY, maxY);

            var sandSource = (x: 500, y: 0);
            var count = 0;
            bool repeat;

            do
            {
                count++;

                repeat = SimulateSand(sandSource, grid);
                //DrawGrid(grid, minX, minY);
            } while (!repeat);
            
            var answer = count - 1;

            Logger.Info($"Day 14A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                    //.ReadAllLines("Content\\Day14_Test.txt")
                    .ReadAllLines("Content\\Day14.txt")
                    .Select(ParseInput)
                    .ToList()
                ;

            var (minX, maxX, minY, maxY) = DetermineGridDimensions(lines);

            //NOTE: Always 0
            minY = 0;

            //NOTE: Calculate the max width by simulating a full stack of sand
            var maxWidth = 1;
            var maxHeight = maxY + 2;
            for (var y = 0; y < maxHeight; y++)
            {
                maxWidth += 2;
            }

            var grid = GenerateGrid(lines, minX, maxX + maxWidth, minY, maxHeight);

            for (var x = 0; x < maxX + maxWidth; x++)
            {
                grid[maxHeight, x] = State.Rock;
            }

            var sandSource = (x: 500, y: 0);
            var count = 0;
            bool repeat;
            do
            {
                count++;

                repeat = SimulateSand(sandSource, grid);
                //DrawGrid(grid, minX - maxWidth, minY);
            } while (!repeat);

            var answer = count - 1;

            Logger.Info($"Day 14B: {answer}");
        }

        private static List<(int x, int y)> ParseInput(string x)
        {
            var lines = x.Split(" -> ");

            return lines
                .Select(y =>
                {
                    var coords = y.Split(",");

                    return (x: int.Parse(coords[0]), y: int.Parse(coords[1]));
                })
                .ToList();
        }

        private static (int minX, int maxX, int minY, int maxY) DetermineGridDimensions(List<List<(int x, int y)>> lines)
        {
            var allCoords = lines.SelectMany(x => x).ToList();

            var minX = allCoords.Min(x => x.x);
            var maxX = allCoords.Max(x => x.x);
            var minY = allCoords.Min(x => x.y);
            var maxY = allCoords.Max(x => x.y);

            return (minX, maxX, minY, maxY);
        }

        private static State[,] GenerateGrid(List<List<(int x, int y)>> lines, int minX, int maxX, int minY, int maxY)
        {
            var grid = new State[maxY + 1, maxX + 1];

            foreach (var line in lines)
            {
                for (var i = 0; i < line.Count - 1; i++)
                {
                    var startCoord = line[i];
                    var endCoord = line[i + 1];

                    if (startCoord.x == endCoord.x)
                    {
                        var lowest = Math.Min(startCoord.y, endCoord.y);
                        var highest = Math.Max(startCoord.y, endCoord.y);

                        for (var y = lowest; y <= highest; y++)
                        {
                            grid[y, startCoord.x] = State.Rock;
                        }
                    }
                    else if (startCoord.y == endCoord.y)
                    {
                        var lowest = Math.Min(startCoord.x, endCoord.x);
                        var highest = Math.Max(startCoord.x, endCoord.x);

                        for (var x = lowest; x <= highest; x++)
                        {
                            grid[startCoord.y, x] = State.Rock;
                        }
                    }
                    else
                    {
                        Logger.Debug("PANIC!");
                    }
                }
            }

            return grid;
        }

        private static bool SimulateSand((int x, int y) sandSource, State[,] grid)
        {
            var sandX = sandSource.x;
            var sandY = sandSource.y;

            if (grid[sandY, sandX] == State.Sand)
            {
                return true;
            }

            retry:
            for (; sandY < grid.GetLength(0); sandY++)
            {
                var current = grid[sandY, sandX];

                if (current == State.Air)
                {
                    continue;
                }

                if (current == State.Rock)
                {
                    goto leftRight;
                }

                break;
            }

            if (sandY == grid.GetLength(0))
            {
                return true;
            }

            leftRight:
            var result = SimulateSandLeftRight((sandX, sandY), grid);

            if (result == 0)
            {
                grid[sandY - 1, sandX] = State.Sand;

                return false;
            }

            sandX = result;

            goto retry;
        }

        private static int SimulateSandLeftRight((int x, int y) sandSource, State[,] grid)
        {
            var sandX = sandSource.x;
            var sandY = sandSource.y;
            
            var left = grid[sandY, sandX - 1];
            var right = grid[sandY, sandX + 1];

            if (left == State.Air)
            {
                return sandX - 1;
            }

            if (right == State.Air)
            {
                return sandX + 1;
            }

            return 0;
        }

        private static void DrawGrid(State[,] grid, int minX, int minY)
        {
            var stringBuilder = new StringBuilder();

            for (var y = minY; y < grid.GetLength(0); y++)
            {
                for (var x = minX; x < grid.GetLength(1); x++)
                {
                    switch (grid[y, x])
                    {
                        case State.Air:
                            stringBuilder.Append('.');
                            break;

                        case State.Rock:
                            stringBuilder.Append('#');
                            break;

                        case State.Sand:
                            stringBuilder.Append('o');
                            break;
                    }

                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder.ToString());
        }
    }
}
