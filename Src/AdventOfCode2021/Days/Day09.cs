using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day09
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day09_Test.txt")
                .ReadAllLines("Content\\Day09.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList();

            List<int> lowPoints = new List<int>();

            for (var y = 0; y < lines.Count; y++)
            {
                List<int> line = lines[y];

                for (int x = 0; x < line.Count; x++)
                {
                    if (IsLower(lines, x, y))
                    {
                        lowPoints.Add(lines[y][x]);
                    }
                }
            }

            int answer = lowPoints.Select(x => x + 1).Sum();

            Logger.Info($"Day 9A: {answer}");
        }

        private static bool IsLower(List<List<int>> lines, int x, int y)
        {
            int value = lines[y][x];

            int p1 = GetValue(lines, x, y - 1);
            int p2 = GetValue(lines, x, y + 1);
            int p3 = GetValue(lines, x - 1, y);
            int p4 = GetValue(lines, x + 1, y);

            if (value < p1 && value < p2 && value < p3 && value < p4)
            {
                return true;
            }

            return false;
        }

        private static int GetValue(List<List<int>> lines, int x, int y)
        {
            if (y < 0 || y >= lines.Count || x < 0 || x >= lines[y].Count)
            {
                return 9;
            }

            return lines[y][x];
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day09_Test.txt")
                .ReadAllLines("Content\\Day09.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList();

            List<int> basins = new List<int>();

            for (var y = 0; y < lines.Count; y++)
            {
                List<int> line = lines[y];

                for (int x = 0; x < line.Count; x++)
                {
                    if (IsLower(lines, x, y))
                    {
                        HashSet<(int, int)> visited = new HashSet<(int, int)> { (x, y) };
                        FindBasin(lines, x, y, visited);
                        basins.Add(visited.Count);
                    }
                }
            }

            int max1 = basins.Max();
            basins.Remove(max1);

            int max2 = basins.Max();
            basins.Remove(max2);

            int max3 = basins.Max();
            basins.Remove(max3);

            int answer = max1 * max2 * max3;

            Logger.Info($"Day 9B: {answer}");
        }

        private static void FindBasin(List<List<int>> lines, int x, int y, HashSet<(int, int)> visited)
        {
            int value = GetValue(lines, x, y);

            int b1 = GetValue(lines, x, y - 1);
            int b2 = GetValue(lines, x, y + 1);
            int b3 = GetValue(lines, x - 1, y);
            int b4 = GetValue(lines, x + 1, y);

            if (b1 < 9 && b1 > value && !visited.Contains((x, y - 1)))
            {
                visited.Add((x, y - 1));

                FindBasin(lines, x, y - 1, visited);
            }

            if (b2 < 9 && b2 > value && !visited.Contains((x, y + 1)))
            {
                visited.Add((x, y + 1));

                FindBasin(lines, x, y + 1, visited);
            }

            if (b3 < 9 && b3 > value && !visited.Contains((x - 1, y)))
            {
                visited.Add((x - 1, y));

                FindBasin(lines, x - 1, y, visited);
            }

            if (b4 < 9 && b4 > value && !visited.Contains((x + 1, y)))
            {
                visited.Add((x + 1, y));

                FindBasin(lines, x + 1, y, visited);
            }
        }
    }
}
