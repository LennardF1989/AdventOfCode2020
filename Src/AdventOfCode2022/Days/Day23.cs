using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day23
    {
        enum Facing
        {
            North = 0,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        }

        private sealed record Point(int x, int y);

        public static void StartA()
        {
            var lines = File
                .ReadAllLines("Content//Day23_Test.txt")
                //.ReadAllLines("Content//Day23.txt")
                .SelectMany((y, i) =>
                {
                    return y
                        .Select((x, i2) => (x, i2, i))
                        .Where(x => x.x == '#')
                        .Select(x => new Point(x.i2, x.i))
                        .ToList();
                })
                .ToList();

            var hashSet = new HashSet<Point>(lines);

            var neighbors = new[]
                {
                    new Point(0, -1), //N
                    new Point(1, -1), //NE
                    new Point(1, 0), //E
                    new Point(1, 1), //SE
                    new Point(0, 1), //S
                    new Point(-1, 1), //SW
                    new Point(-1, 0), //W
                    new Point(-1, -1), //NW
                }
                .Select((x, i) => ((Facing) i, x))
                .ToDictionary(x => x.Item1, x => x.x);

            var groups = new Dictionary<Facing, Point[]>()
            {
                {
                    Facing.North, new[]
                    {
                        neighbors[Facing.North],
                        neighbors[Facing.NorthEast],
                        neighbors[Facing.NorthWest],
                    }
                },
                {
                    Facing.South, new[]
                    {
                        neighbors[Facing.South],
                        neighbors[Facing.SouthEast],
                        neighbors[Facing.SouthWest],
                    }
                },
                {
                    Facing.West, new[]
                    {
                        neighbors[Facing.West],
                        neighbors[Facing.NorthWest],
                        neighbors[Facing.SouthWest],
                    }
                },
                {
                    Facing.East, new[]
                    {
                        neighbors[Facing.East],
                        neighbors[Facing.NorthEast],
                        neighbors[Facing.SouthEast]
                    }
                }
            };

            for (var i = 0; i < 10; i++)
            {
                var proposedMoves = new Dictionary<Point, Point>();
                Facing? firstFacing = null;
                
                //Part 1
                foreach (var point in hashSet)
                {
                    var noNeighbors = false;
                    var lastGroup = Facing.North;
                    
                    foreach (var group in groups)
                    {
                        noNeighbors = true;
                        lastGroup = group.Key;
                    
                        foreach (var neighbor in group.Value)
                        {
                            var p = new Point(point.x + neighbor.x, point.y + neighbor.y);

                            if (hashSet.Contains(p))
                            {
                                noNeighbors = false;
                                break;
                            }
                        }
                    }
                
                    if (noNeighbors)
                    {
                        if (!firstFacing.HasValue)
                        {
                            firstFacing = lastGroup;
                        }
                    
                        var direction = neighbors[lastGroup];
                        proposedMoves.Add(point, new Point(point.x + direction.x, point.y + direction.y));
                    }
                    
                    //Each elf has its own list.
                }
                
                //Part 2
                proposedMoves.Clear();
            }

            var answer = 0;

            Logger.Info($"Day 23A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                    .ReadAllLines("Content//Day23_Test.txt")
                //.ReadAllLines("Content//Day23.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 23B: {answer}");
        }
    }
}