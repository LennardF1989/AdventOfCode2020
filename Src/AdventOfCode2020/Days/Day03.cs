using System.IO;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day03
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day03_Test.txt");
            var lines = File.ReadAllLines("Content\\Day03.txt");

            int maxY = lines.Length;
            int maxX = lines[0].Length;

            //Create grid
            bool[,] grid = new bool[maxY, maxX];

            for (var y = 0; y < lines.Length; y++)
            {
                string line = lines[y];

                for (var x = 0; x < line.Length; x++)
                {
                    char c = line[x];
                    grid[y, x] = c == '#';
                }
            }

            //Check the grid
            int slopeRight = 3;
            int slopeDown = 1;

            var trees = CountTrees(grid, maxY, maxX, slopeRight, slopeDown);

            Logger.Info($"Day 3A: {trees}");
        }

        private static int CountTrees(bool[,] grid, int maxY, int maxX, int slopeRight, int slopeDown)
        {
            int currentX = 0;
            int currentY = 0;

            int trees = 0;

            while (true)
            {
                currentX = (currentX + slopeRight) % maxX;
                currentY = (currentY + slopeDown);

                if (currentY >= maxY)
                {
                    break;
                }

                if (grid[currentY, currentX])
                {
                    trees++;
                }
            }

            return trees;
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day03_Test.txt");
            var lines = File.ReadAllLines("Content\\Day03.txt");

            int maxY = lines.Length;
            int maxX = lines[0].Length;

            //Create grid
            bool[,] grid = new bool[maxY, maxX];

            for (var y = 0; y < lines.Length; y++)
            {
                string line = lines[y];

                for (var x = 0; x < line.Length; x++)
                {
                    char c = line[x];
                    grid[y, x] = c == '#';
                }
            }

            long trees1 = CountTrees(grid, maxY, maxX, 1, 1);
            long trees2 = CountTrees(grid, maxY, maxX, 3, 1);
            long trees3 = CountTrees(grid, maxY, maxX, 5, 1);
            long trees4 = CountTrees(grid, maxY, maxX, 7, 1);
            long trees5 = CountTrees(grid, maxY, maxX, 1, 2);

            Logger.Debug(trees1);
            Logger.Debug(trees2);
            Logger.Debug(trees3);
            Logger.Debug(trees4);
            Logger.Debug(trees5);

            Logger.Info($"Day 3B: {trees1 * trees2 * trees3 * trees4 * trees5}");
        }
    }
}
