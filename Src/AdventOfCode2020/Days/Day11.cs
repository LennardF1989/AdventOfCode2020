using System.IO;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day11
    {
        public static void StartA()
        {
            var grid = GetGrid();

            while (true)
            {
                var previousGrid = grid;

                grid = IterateSeats(grid, 4, false);

                if (CompareGrid(previousGrid, grid))
                {
                    break;
                }
            }

            int answer = CountOccupiedSeats(grid);

            Logger.Info($"Day 11A: {answer}");
        }

        public static void StartB()
        {
            var grid = GetGrid();

            while (true)
            {
                var previousGrid = grid;

                grid = IterateSeats(grid, 5, true);

                if (CompareGrid(previousGrid, grid))
                {
                    break;
                }
            }

            int answer = CountOccupiedSeats(grid);

            Logger.Info($"Day 11B: {answer}");
        }

        private static char[,] GetGrid()
        {
            //var lines = File.ReadAllLines("Content\\Day11_Test.txt");
            var lines = File.ReadAllLines("Content\\Day11.txt");

            char[,] grid = new char[lines.Length, lines[0].Length];
            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];

                for (var x = 0; x < line.Length; x++)
                {
                    char c = line[x];

                    grid[y, x] = c;
                }
            }

            return grid;
        }

        private static char[,] IterateSeats(char[,] grid, int numberOfOccupiedSeats, bool scan)
        {
            char[,] gridCopy = new char[grid.GetLength(0), grid.GetLength(1)];

            for (var y = 0; y < grid.GetLength(0); y++)
            {
                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    char c = grid[y, x];

                    if (c == 'L')
                    {
                        int count = 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, -1, -1, scan) ? 0 : 1;
                        count += GetOccupiedNeighbourScan(grid, y, x, -1, 0, scan) ? 0 : 1;
                        count += GetOccupiedNeighbourScan(grid, y, x, -1, 1, scan) ? 0 : 1;
                        count += GetOccupiedNeighbourScan(grid, y, x, 0, -1, scan) ? 0 : 1;
                        count += GetOccupiedNeighbourScan(grid, y, x, 0, 1, scan) ? 0 : 1;
                        count += GetOccupiedNeighbourScan(grid, y, x, 1, -1, scan) ? 0 : 1;
                        count += GetOccupiedNeighbourScan(grid, y, x, 1, 0, scan) ? 0 : 1;
                        count += GetOccupiedNeighbourScan(grid, y, x, 1, 1, scan) ? 0 : 1;

                        if (count == 8)
                        {
                            gridCopy[y, x] = '#';
                        }
                        else
                        {
                            gridCopy[y, x] = 'L';
                        }
                    }
                    else if (c == '#')
                    {
                        int count = 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, -1, -1, scan) ? 1 : 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, -1, 0, scan) ? 1 : 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, -1, 1, scan) ? 1 : 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, 0, -1, scan) ? 1 : 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, 0, 1, scan) ? 1 : 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, 1, -1, scan) ? 1 : 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, 1, 0, scan) ? 1 : 0;
                        count += GetOccupiedNeighbourScan(grid, y, x, 1, 1, scan) ? 1 : 0;

                        if (count >= numberOfOccupiedSeats)
                        {
                            gridCopy[y, x] = 'L';
                        }
                        else
                        {
                            gridCopy[y, x] = '#';
                        }
                    }
                    else
                    {
                        gridCopy[y, x] = c;
                    }
                }
            }

            PrintGrid(gridCopy);

            return gridCopy;
        }

        private static bool GetOccupiedNeighbourScan(char[,] grid, int y, int x, int deltaY, int deltaX, bool scan)
        {
            while (true)
            {
                x += deltaX;
                y += deltaY;

                if (x < 0 || x >= grid.GetLength(1) || y < 0 || y >= grid.GetLength(0))
                {
                    return false;
                }

                if (grid[y, x] == 'L')
                {
                    return false;
                }

                if (grid[y, x] == '#')
                {
                    return true;
                }

                if (!scan)
                {
                    return false;
                }
            }
        }

        private static int CountOccupiedSeats(char[,] grid)
        {
            int count = 0;

            for (var y = 0; y < grid.GetLength(0); y++)
            {
                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] == '#')
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static bool CompareGrid(char[,] previousGrid, char[,] grid)
        {
            for (var y = 0; y < grid.GetLength(0); y++)
            {
                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    if (previousGrid[y, x] != grid[y, x])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void PrintGrid(char[,] grid)
        {
            for (var y = 0; y < grid.GetLength(0); y++)
            {
                string line = string.Empty;

                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    line += grid[y, x];
                }

                Logger.Debug(line);
            }

            Logger.Debug("");
        }
    }
}
