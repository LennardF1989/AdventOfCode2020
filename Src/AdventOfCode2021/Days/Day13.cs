using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day13
    {
        public static void Start()
        {
            var lines = File
                .ReadAllText("Content\\Day13_Test.txt")
                //.ReadAllText("Content\\Day13.txt")
                .Split("\r\n\r\n");

            var points = lines[0]
                    .Split("\r\n")
                    .Select(x =>
                    {
                        var point = x
                            .Split(",")
                            .Select(int.Parse)
                            .ToList();

                        return (x: point[0], y: point[1]);
                    })
                    .ToList();

            var folds = lines[1]
                .Split("\r\n")
                .Select(x =>
                {
                    var split = x.Split("=");

                    return (axis: split[0][^1], value: int.Parse(split[1]));
                })
                .ToList();

            PrepareSheet(points, folds);
        }

        private static void PrepareSheet(List<(int x, int y)> points, List<(char axis, int value)> folds)
        {
            int maxX = points.Max(x => x.x);
            int maxY = points.Max(x => x.y);

            var grid = new List<List<char>>();

            for (int y = 0; y <= maxY; y++)
            {
                grid.Add(new List<char>());

                grid[y].AddRange(Enumerable.Repeat('.', maxX + 1));
            }

            points.ForEach(p =>
            {
                grid[p.y][p.x] = '#';
            });

            FoldSheet(grid, folds.Take(1).ToList());

            int answer = grid.Sum(x => x.Count(y => y == '#'));

            Logger.Info($"Day 13A: {answer}");

            FoldSheet(grid, folds.Skip(1).ToList());

            Logger.Info("Day 13B:");
            PrintGrid(grid);
        }

        private static void FoldSheet(List<List<char>> grid, List<(char axis, int value)> folds)
        {
            foreach (var tuple in folds)
            {
                if (tuple.axis == 'x')
                {
                    for (int y = 0; y < grid.Count; y++)
                    {
                        //NOTE: For debugging
                        grid[y][tuple.value] = '|';

                        for (int x = tuple.value; x < grid[y].Count; x++)
                        {
                            char temp = grid[y][x];

                            if (temp == '#')
                            {
                                grid[y][x + ((tuple.value - x) * 2)] = temp;
                            }
                        }

                        int amount = grid[y].Count - tuple.value;

                        for (int i = 0; i < amount; i++)
                        {
                            grid[y].RemoveAt(grid[y].Count - 1);
                        }
                    }
                }
                else if (tuple.axis == 'y')
                {
                    //NOTE: For debugging
                    for (int i = 0; i < grid[tuple.value].Count; i++)
                    {
                        grid[tuple.value][i] = '-';
                    }

                    for (int y = tuple.value; y < grid.Count; y++)
                    {
                        for (int x = 0; x < grid[y].Count; x++)
                        {
                            char temp = grid[y][x];

                            if (temp == '#')
                            {
                                grid[y + ((tuple.value - y) * 2)][x] = temp;
                            }
                        }
                    }

                    int amount = grid.Count - tuple.value;

                    for (int i = 0; i < amount; i++)
                    {
                        grid.RemoveAt(grid.Count - 1);
                    }
                }
            }
        }

        private static void PrintGrid(List<List<char>> lines)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Count; x++)
                {
                    char value = lines[y][x];

                    stringBuilder.Append(value);
                }

                stringBuilder.AppendLine();
            }
            
            Logger.Info(stringBuilder.ToString());
        }
    }
}