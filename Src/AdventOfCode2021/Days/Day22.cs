using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day22
    {
        public static void StartA()
        {
            var lines = ParseInput("Content\\Day22.txt");
            
            var result = PerformInstructions(lines, -50, -50, -50, 50, 50, 50);
            
            int answer = result.Count;

            Logger.Info($"Day 22A: {answer}");
        }

        public static void StartB()
        {
            //var lines = ParseInput("Content\\Day22_Test2.txt");
            var lines = ParseInput("Content\\Day22.txt");
            
            var answer = PerformInstructions2(lines);

            Logger.Info($"Day 22B: {answer}");
        }

        private static List<(bool onOff, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)> ParseInput(string fileName)
        {
            var lines = File
                .ReadAllLines(fileName)
                .Select(x =>
                {
                    var instructions = x.Split(" ");

                    var onOff = instructions[0] == "on";
                    var ranges = instructions[1].Split(",");

                    var cx = ranges[0].Split("=")[1].Split("..").Select(int.Parse).ToList();
                    var cy = ranges[1].Split("=")[1].Split("..").Select(int.Parse).ToList();
                    var cz = ranges[2].Split("=")[1].Split("..").Select(int.Parse).ToList();

                    return (onOff: onOff, minX: cx[0], maxX: cx[1], minY: cy[0], maxY: cy[1], minZ: cz[0], maxZ: cz[1]);
                })
                .ToList();
            return lines;
        }

        private static HashSet<(int, int, int)> PerformInstructions(
            List<(bool onOff, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)> lines,
            int minX, int minY, int minZ,
            int maxX, int maxY, int maxZ
        )
        {
            var cuboids = new HashSet<(int, int, int)>();

            foreach (var instruction in lines)
            {
                for (int z = instruction.minZ; z <= instruction.maxZ; z++)
                {
                    if (z < minZ || z > maxZ)
                    {
                        continue;
                    }

                    for (int y = instruction.minY; y <= instruction.maxY; y++)
                    {
                        if (y < minY || y > maxY)
                        {
                            continue;
                        }

                        for (int x = instruction.minX; x <= instruction.maxX; x++)
                        {
                            if (x < minX || x > maxX)
                            {
                                continue;
                            }

                            if (instruction.onOff)
                            {
                                cuboids.Add((x, y, z));
                            }
                            else
                            {
                                cuboids.Remove((x, y, z));
                            }
                        }
                    }
                }
            }

            return cuboids;
        }

        private static long PerformInstructions2(
            List<(bool onOff, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)> lines
        )
        {
            var instructions = lines
                .Select(x => new Instruction
                {
                    MinX = x.minX,
                    MinY = x.minY,
                    MinZ = x.minZ,
                    MaxX = x.maxX + 1,
                    MaxY = x.maxY + 1,
                    MaxZ = x.maxZ + 1,
                    OnOff = x.onOff
                })
                .ToList();

            var reactor = new Reactor();

            foreach (var instruction in instructions)
            {
                if (instruction.OnOff)
                {
                    reactor.Add(instruction);
                }
                else
                {
                    reactor.Remove(instruction);
                }
            }

            return reactor.Count();
        }
    }

    class Reactor
    {
        public List<Cuboid> Cuboids { get; set; }

        public Reactor()
        {
            Cuboids = new List<Cuboid>();
        }

        public void Add(Cuboid cuboid)
        {
            var incomingCubes = new List<Cuboid>
            {
                cuboid
            };

            foreach (var thisCube in Cuboids)
            {
                var newCubes = new List<Cuboid>();

                foreach (var incomingCube in incomingCubes)
                {
                    if (thisCube.Contains(incomingCube))
                    {
                        continue;
                    }

                    if (!incomingCube.Intersects(thisCube))
                    {
                        newCubes.Add(incomingCube);

                        continue;
                    }

                    var splitCubes = incomingCube.Split(thisCube);
                    //var splitCubes = incomingCube.Split2(thisCube);
                    newCubes.AddRange(splitCubes);
                }

                incomingCubes = newCubes;
            }

            Cuboids.AddRange(incomingCubes);
        }

        public void Remove(Cuboid cuboid)
        {
            var newCubes = new List<Cuboid>();

            foreach (var thisCube in Cuboids)
            {
                if (!cuboid.Intersects(thisCube))
                {
                    newCubes.Add(thisCube);

                    continue;
                }

                if (cuboid.Contains(thisCube))
                {
                    continue;
                }

                var splitCubes = thisCube.Split(cuboid);
                //var splitCubes = thisCube.Split2(cuboid);
                newCubes.AddRange(splitCubes);
            }

            Cuboids = newCubes;
        }

        public long Count()
        {
            return Cuboids.Sum(x => (long)x.Width * x.Depth * x.Height);
        }
    }

    class Instruction : Cuboid
    {
        public bool OnOff { get; set; }
    }

    class Cuboid
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }
        public int MinZ { get; set; }
        public int MaxZ { get; set; }

        public int Width => Math.Abs(MaxX - MinX);
        public int Depth => Math.Abs(MaxY - MinY);
        public int Height => Math.Abs(MaxZ - MinZ);

        public bool Contains(Cuboid other)
        {
            return MinX <= other.MinX && MaxX >= other.MaxX &&
                   MinY <= other.MinY && MaxY >= other.MaxY &&
                   MinZ <= other.MinZ && MaxZ >= other.MaxZ;
        }

        public bool Intersects(Cuboid other)
        {
            return MinX < other.MaxX && MaxX > other.MinX &&
                   MinY < other.MaxY && MaxY > other.MinY &&
                   MinZ < other.MaxZ && MaxZ > other.MinZ;
        }

        public Cuboid Intersection(Cuboid other)
        {
            var maxX = Math.Min(MaxX, other.MaxX);
            var minX = Math.Max(MinX, other.MinX);

            var maxY = Math.Min(MaxY, other.MaxY);
            var minY = Math.Max(MinY, other.MinY);

            var maxZ = Math.Min(MaxZ, other.MaxZ);
            var minZ = Math.Max(MinZ, other.MinZ);

            var diffX = Math.Max(maxX - minX, 0);
            var diffY = Math.Max(maxY - minY, 0);
            var diffZ = Math.Max(maxZ - minZ, 0);

            if (diffX == 0 && diffY == 0 && diffZ == 0)
            {
                return null;
            }

            return new Cuboid
            {
                MinX = minX,
                MinY = minY,
                MinZ = minZ,
                MaxX = maxX,
                MaxY = maxY,
                MaxZ = maxZ
            };
        }

        public List<Cuboid> Split(Cuboid other)
        {
            var result = Intersection(other);

            if (result == null)
            {
                return null;
            }

            var vertices = new List<(int, int, int, int, int, int)>
            {
                //Bottom
                (MinX       , MinY       , MinZ       , result.MinX, result.MinY, result.MinZ),
                (MinX       , result.MinY, MinZ       , result.MinX, result.MaxY, result.MinZ),
                (MinX       , result.MaxY, MinZ       , result.MinX, MaxY       , result.MinZ),
                (result.MinX, MinY       , MinZ       , result.MaxX, result.MinY, result.MinZ),
                (result.MinX, result.MinY, MinZ       , result.MaxX, result.MaxY, result.MinZ),
                (result.MinX, result.MaxY, MinZ       , result.MaxX, MaxY       , result.MinZ),
                (result.MaxX, MinY       , MinZ       , MaxX       , result.MinY, result.MinZ),
                (result.MaxX, result.MinY, MinZ       , MaxX       , result.MaxY, result.MinZ),
                (result.MaxX, result.MaxY, MinZ       , MaxX       , MaxY       , result.MinZ),
                //Middle
                (MinX       , MinY       , result.MinZ, result.MinX, result.MinY, result.MaxZ),
                (MinX       , result.MinY, result.MinZ, result.MinX, result.MaxY, result.MaxZ),
                (MinX       , result.MaxY, result.MinZ, result.MinX, MaxY       , result.MaxZ),
                (result.MinX, MinY       , result.MinZ, result.MaxX, result.MinY, result.MaxZ),
                //(result.MinX, result.MinY, result.MinZ, result.MaxX, result.MaxY, result.MaxZ),
                (result.MinX, result.MaxY, result.MinZ, result.MaxX, MaxY       , result.MaxZ),
                (result.MaxX, MinY       , result.MinZ, MaxX       , result.MinY, result.MaxZ),
                (result.MaxX, result.MinY, result.MinZ, MaxX       , result.MaxY, result.MaxZ),
                (result.MaxX, result.MaxY, result.MinZ, MaxX       , MaxY       , result.MaxZ),
                //Top
                (MinX       , MinY       , result.MaxZ, result.MinX, result.MinY, MaxZ),
                (MinX       , result.MinY, result.MaxZ, result.MinX, result.MaxY, MaxZ),
                (MinX       , result.MaxY, result.MaxZ, result.MinX, MaxY       , MaxZ),
                (result.MinX, MinY       , result.MaxZ, result.MaxX, result.MinY, MaxZ),
                (result.MinX, result.MinY, result.MaxZ, result.MaxX, result.MaxY, MaxZ),
                (result.MinX, result.MaxY, result.MaxZ, result.MaxX, MaxY       , MaxZ),
                (result.MaxX, MinY       , result.MaxZ, MaxX       , result.MinY, MaxZ),
                (result.MaxX, result.MinY, result.MaxZ, MaxX       , result.MaxY, MaxZ),
                (result.MaxX, result.MaxY, result.MaxZ, MaxX       , MaxY       , MaxZ),
            };

            var cuboids = vertices
                .Where(x => x.Item4 - x.Item1 != 0 && x.Item5 - x.Item2 != 0 && x.Item6 - x.Item3 != 0)
                .Select(x => new Cuboid
                {
                    MinX = x.Item1,
                    MinY = x.Item2,
                    MinZ = x.Item3,
                    MaxX = x.Item4,
                    MaxY = x.Item5,
                    MaxZ = x.Item6
                })
                .ToList();

            return cuboids.Any() 
                ? cuboids
                : null;
        }

        //Disclaimer: This version is inspired by someone else.
        public List<Cuboid> Split2(Cuboid other)
        {
            var newCubes = new List<Cuboid>();
            var cutsX = new[] { MinX, MaxX, other.MinX, other.MaxX };
            var cutsY = new[] { MinY, MaxY, other.MinY, other.MaxY };
            var cutsZ = new[] { MinZ, MaxZ, other.MinZ, other.MaxZ };
            Array.Sort(cutsX);
            Array.Sort(cutsY);
            Array.Sort(cutsZ);

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    for (var z = 0; z < 3; z++)
                    {
                        var newCube = new Cuboid
                        {
                            MinX = cutsX[x],
                            MaxX = cutsX[x + 1],
                            MinY = cutsY[y],
                            MaxY = cutsY[y + 1],
                            MinZ = cutsZ[z],
                            MaxZ = cutsZ[z + 1]
                        };

                        if (newCube.Width != 0 && 
                            newCube.Depth != 0 && 
                            newCube.Height != 0 &&
                            Contains(newCube) && 
                            !other.Contains(newCube))
                        {
                            newCubes.Add(newCube);
                        }
                    }
                }
            }

            return newCubes;
        }

        public override string ToString()
        {
            return $"{Width}x{Depth}x{Height}";
        }
    }
}
