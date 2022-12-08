using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day08
    {
        public static void StartA()
        {
            var grid = File
                //.ReadAllLines("Content\\Day08_Test.txt")
                .ReadAllLines("Content\\Day08.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList()
                ;

            var totalVisible = (grid.Count * 2) + (grid[0].Count * 2) - 4;

            for (var y = 1; y < grid.Count - 1; y++)
            {
                for (var x = 1; x < grid[0].Count - 1; x++)
                {
                    var result = ScanLeft(grid, x, y).Item1 ||
                                 ScanRight(grid, x, y).Item1 ||
                                 ScanUp(grid, x, y).Item1 ||
                                 ScanDown(grid, x, y).Item1;

                    if (result)
                    {
                        totalVisible++;
                    }
                }
            }

            var answer = totalVisible;

            Logger.Info($"Day 8A: {answer}");
        }

        public static void StartB()
        {
            var grid = File
                //.ReadAllLines("Content\\Day08_Test.txt")
                .ReadAllLines("Content\\Day08.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList()
                ;

            var bestScore = 0;

            for (var y = 1; y < grid.Count - 1; y++)
            {
                for (var x = 1; x < grid[0].Count - 1; x++)
                {
                    var scoreLeft = ScanLeft(grid, x, y).Item2;
                    var scoreRight = ScanRight(grid, x, y).Item2;
                    var scoreUp = ScanUp(grid, x, y).Item2;
                    var scoreDown = ScanDown(grid, x, y).Item2;

                    var score = scoreLeft * scoreRight * scoreDown * scoreUp;

                    if (score > bestScore)
                    {
                        bestScore = score;
                    }
                }
            }

            var answer = bestScore;

            Logger.Info($"Day 8B: {answer}");
        }

        private static (bool, int) ScanLeft(List<List<int>> grid, int x, int y)
        {
            var current = grid[y][x];
            var count = 0;

            for (var i = x - 1; i >= 0; i--)
            {
                count++;

                if (grid[y][i] >= current)
                {
                    return (false, count);
                }
            }

            return (true, count);
        }

        private static (bool, int) ScanRight(List<List<int>> grid, int x, int y)
        {
            var current = grid[y][x];
            var count = 0;

            for (var i = x + 1; i < grid[y].Count; i++)
            {
                count++;

                if (grid[y][i] >= current)
                {
                    return (false, count);
                }
            }

            return (true, count);
        }

        private static (bool, int) ScanUp(List<List<int>> grid, int x, int y)
        {
            var current = grid[y][x];
            var count = 0;

            for (var i = y - 1; i >= 0; i--)
            {
                count++;

                if (grid[i][x] >= current)
                {
                    return (false, count);
                }
            }

            return (true, count);
        }

        private static (bool, int) ScanDown(List<List<int>> grid, int x, int y)
        {
            var current = grid[y][x];
            var count = 0;

            for (var i = y + 1; i < grid.Count; i++)
            {
                count++;

                if (grid[i][x] >= current)
                {
                    return (false, count);
                }
            }

            return (true, count);
        }
    }
}
