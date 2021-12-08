using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2019.Days
{
    public static class Day03
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test.txt")
                .ReadAllLines("Content\\Day03.txt")
                .Select(x => x
                    .Split(",")
                    .Select(y => (direction: y[0], amount: int.Parse(y.Substring(1))))
                    .ToList())
                .ToList();

            var grid = new List<HashSet<(int x, int y)>>();

            for (var index = 0; index < lines.Count; index++)
            {
                grid.Add(new HashSet<(int x, int y)>());

                var hashSet = grid[index];
                var line = lines[index];

                int x = 0;
                int y = 0;

                foreach (var instruction in line)
                {
                    switch (instruction.direction)
                    {
                        case 'U':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                y++;

                                hashSet.Add((x, y));
                            }

                            break;

                        case 'D':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                y--;

                                hashSet.Add((x, y));
                            }

                            break;

                        case 'L':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                x--;

                                hashSet.Add((x, y));
                            }

                            break;

                        case 'R':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                x++;

                                hashSet.Add((x, y));
                            }

                            break;
                    }
                }
            }

            var overlaps = grid[0]
                .Intersect(grid[1])
                .ToList();

            int answer = overlaps
                .Select(x => Math.Abs(x.x) + Math.Abs(x.y))
                .Min();

            Logger.Info($"Day 3A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test.txt")
                .ReadAllLines("Content\\Day03.txt")
                .Select(x => x
                    .Split(",")
                    .Select(y => (direction: y[0], amount: int.Parse(y.Substring(1))))
                    .ToList())
                .ToList();

            var grid = new List<Dictionary<(int x, int y), int>>();

            for (var index = 0; index < lines.Count; index++)
            {
                grid.Add(new Dictionary<(int x, int y), int>());

                var dictionary = grid[index];
                var line = lines[index];

                int x = 0;
                int y = 0;
                int steps = 0;

                foreach (var instruction in line)
                {
                    switch (instruction.direction)
                    {
                        case 'U':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                steps++;
                                y++;

                                if(!dictionary.ContainsKey((x, y)))
                                {
                                    dictionary.Add((x, y), steps);
                                }
                            }

                            break;

                        case 'D':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                steps++;
                                y--;

                                if(!dictionary.ContainsKey((x, y)))
                                {
                                    dictionary.Add((x, y), steps);
                                }
                            }

                            break;

                        case 'L':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                steps++;
                                x--;

                                if(!dictionary.ContainsKey((x, y)))
                                {
                                    dictionary.Add((x, y), steps);
                                }
                            }

                            break;

                        case 'R':
                            for (int i = 0; i < instruction.amount; i++)
                            {
                                steps++;
                                x++;

                                if(!dictionary.ContainsKey((x, y)))
                                {
                                    dictionary.Add((x, y), steps);
                                }
                            }

                            break;
                    }
                }
            }

            var overlaps = grid[0].Keys
                .Intersect(grid[1].Keys)
                .ToList();

            int lowestSum = int.MaxValue;

            foreach (var coord in overlaps)
            {
                int sum = grid[0][coord] + grid[1][coord];

                if (sum < lowestSum)
                {
                    lowestSum = sum;
                }
            }

            Logger.Info($"Day 3B: {lowestSum}");
        }
    }
}
