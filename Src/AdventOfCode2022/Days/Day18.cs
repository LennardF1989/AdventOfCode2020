using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day18
    {
        private sealed record Cube(int X, int Y, int Z);

        private static readonly List<Cube> _neighbors = new()
        {
            new(-1, 0, 0),
            new(1, 0, 0),
            new(0, -1, 0),
            new(0, 1, 0),
            new(0, 0, -1),
            new(0, 0, 1)
        };

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day18_Test.txt")
                .ReadAllLines("Content\\Day18.txt")
                .Select(ParseInput)
                .ToHashSet();

            var totalSides = lines.Count * 6;

            foreach (var cube in lines)
            {
                foreach (var neighbor in _neighbors)
                {
                    var result = lines.Contains(
                        new Cube(cube.X + neighbor.X, cube.Y + neighbor.Y, cube.Z + neighbor.Z)
                    );

                    if (result)
                    {
                        totalSides--;
                    }
                }
            }

            var answer = totalSides;

            Logger.Info($"Day 18A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day18_Test.txt")
                .ReadAllLines("Content\\Day18.txt")
                .Select(ParseInput)
                .ToHashSet();

            //Add a some space around droplet
            var minX = lines.Min(x => x.X) - 1;
            var minY = lines.Min(x => x.Y) - 1;
            var minZ = lines.Min(x => x.Z) - 1;
            var maxX = lines.Max(x => x.X) + 1;
            var maxY = lines.Max(x => x.Y) + 1;
            var maxZ = lines.Max(x => x.Z) + 1;

            //Start from one corner
            var steam = new Queue<Cube>();
            steam.Enqueue(new Cube(minX, minY, minZ));

            var visited = new HashSet<Cube>();
            var totalSides = 0;

            while (steam.Count > 0)
            {
                var cube = steam.Dequeue();

                //Expand the steam
                foreach (var neighbor in _neighbors)
                {
                    var newCube = new Cube(cube.X + neighbor.X, cube.Y + neighbor.Y, cube.Z + neighbor.Z);

                    if (visited.Contains(newCube))
                    {
                        continue;
                    }

                    //If we go out of bounds, treat it as visited so skip it next time.
                    if (
                        newCube.X < minX || newCube.Y < minY || newCube.Z < minZ ||
                        newCube.X > maxX || newCube.Y > maxY || newCube.Z > maxZ
                    )
                    {
                        visited.Add(newCube);

                        continue;
                    }

                    //If this cube was part of the droplet, we found a side.
                    if (lines.Contains(newCube))
                    {
                        totalSides++;

                        continue;
                    }

                    //We mark as visited now, otherwise we also exclude cubes part of the droplet.
                    visited.Add(newCube);

                    steam.Enqueue(newCube);
                }
            }

            var answer = totalSides;

            Logger.Info($"Day 18B: {answer}");
        }

        private static Cube ParseInput(string x)
        {
            var split = x.Split(",");

            return new Cube(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
        }
    }
}
