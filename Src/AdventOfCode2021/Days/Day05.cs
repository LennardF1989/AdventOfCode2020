using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Days
{
    public static class Day05
    {
        public static void StartA()
        {
            //var amount = GetOverlappingAmount("Content\\Day05_Test.txt", false);
            var amount = GetOverlappingAmount("Content\\Day05.txt", false);

            Logger.Info($"Answer 5A: {amount}");
        }

        public static void StartB()
        {
            //var amount = GetOverlappingAmount("Content\\Day05_Test.txt", true);
            var amount = GetOverlappingAmount("Content\\Day05.txt", true);

            Logger.Info($"Answer 5B: {amount}");
        }

        private static int GetOverlappingAmount(string fileName, bool includeDiagonal)
        {
            var lines = File
                    .ReadAllLines(fileName)
                    .Select(x =>
                    {
                        var points = x.Split(" -> ");

                        var p1 = points[0].Split(",").Select(int.Parse).ToList();
                        var p2 = points[1].Split(",").Select(int.Parse).ToList();

                        return (new Point(p1[0], p1[1]), new Point(p2[0], p2[1]));
                    })
                    .ToList()
                ;

            //Get bounding box
            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;

            foreach ((Point, Point) line in lines)
            {
                if (maxX < line.Item1.X)
                {
                    maxX = line.Item1.X;
                }

                if (maxX < line.Item2.X)
                {
                    maxX = line.Item2.X;
                }

                if (maxY < line.Item1.Y)
                {
                    maxY = line.Item1.Y;
                }

                if (maxY < line.Item2.Y)
                {
                    maxY = line.Item2.Y;
                }
            }

            int[,] grid = new int[maxY + 1, maxX + 1];

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                //Diagonal
                if (line.Item1.X != line.Item2.X && line.Item1.Y != line.Item2.Y)
                {
                    if (!includeDiagonal)
                    {
                        continue;
                    }

                    var leftX = (line.Item1.X < line.Item2.X) ? line.Item1 : line.Item2;
                    var rightX = (leftX == line.Item1) ? line.Item2 : line.Item1;

                    //LeftTop to BottomRight
                    if (leftX.Y < rightX.Y)
                    {
                        int x = leftX.X;
                        int y = leftX.Y;

                        do
                        {
                            grid[y, x]++;

                            x++;
                            y++;
                        } while (x != rightX.X && y != rightX.Y);

                        grid[y, x]++;

                        continue;
                    }

                    //BottomLeft to TopRight
                    if (leftX.Y > rightX.Y)
                    {
                        int x = leftX.X;
                        int y = leftX.Y;

                        do
                        {
                            grid[y, x]++;

                            x++;
                            y--;
                        } while (x != rightX.X && y != rightX.Y);

                        grid[y, x]++;

                        continue;
                    }
                }

                //Vertical
                if (line.Item1.X == line.Item2.X)
                {
                    var left = (line.Item1.Y < line.Item2.Y) ? line.Item1 : line.Item2;
                    var right = (left == line.Item1) ? line.Item2 : line.Item1;

                    for (int y = left.Y; y <= right.Y; y++)
                    {
                        grid[y, left.X]++;
                    }

                    continue;
                }

                //Horizontal
                if (line.Item1.Y == line.Item2.Y)
                {
                    var left = (line.Item1.X < line.Item2.X) ? line.Item1 : line.Item2;
                    var right = (left == line.Item1) ? line.Item2 : line.Item1;

                    for (int x = left.X; x <= right.X; x++)
                    {
                        grid[left.Y, x]++;
                    }

                    continue;
                }
            }

            int amount = 0;

            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] >= 2)
                    {
                        amount++;
                    }
                }
            }

            return amount;
        }
    }
}
