using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                //.ReadAllLines("Content//Day23_Test.txt")
                //.ReadAllLines("Content//Day23_Test2.txt")
                .ReadAllLines("Content//Day23.txt");

            var input = ParseInput(lines);
            var (elfs, _) = SimulateRound(input, 10);

            //DrawGrid(elfs);

            var minX = elfs.Min(x => x.x);
            var maxX = elfs.Max(x => x.x);
            var minY = elfs.Min(x => x.y);
            var maxY = elfs.Max(x => x.y);

            var answer = ((maxY - minY + 1) * (maxX - minX + 1)) - elfs.Count;

            Logger.Info($"Day 23A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content//Day23_Test.txt")
                //.ReadAllLines("Content//Day23_Test2.txt")
                .ReadAllLines("Content//Day23.txt");

            var input = ParseInput(lines);
            var (elfs, round) = SimulateRound(input, 0);

            //DrawGrid(elfs);

            var answer = round;

            Logger.Info($"Day 23B: {answer}");
        }

        private static List<Point> ParseInput(string[] lines)
        {
            return lines.SelectMany((y, i) =>
                {
                    return y
                        .Select((x, i2) => (x, i2, i))
                        .Where(x => x.x == '#')
                        .Select(x => new Point(x.i2, x.i))
                        .ToList();
                })
                .ToList();
        }

        private static (HashSet<Point> elfs, int round) SimulateRound(List<Point> input, int maxRounds)
        {
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
                .Select((x, i) => (facing: (Facing)i, point: x))
                .ToDictionary(x => x.facing, x => x.point);

            var groups = new Dictionary<Facing, Point[]>
            {
                {
                    Facing.North, new[]
                    {
                        neighbors[Facing.North],
                        neighbors[Facing.NorthEast],
                        neighbors[Facing.NorthWest]
                    }
                },
                {
                    Facing.South, new[]
                    {
                        neighbors[Facing.South],
                        neighbors[Facing.SouthEast],
                        neighbors[Facing.SouthWest]
                    }
                },
                {
                    Facing.West, new[]
                    {
                        neighbors[Facing.West],
                        neighbors[Facing.NorthWest],
                        neighbors[Facing.SouthWest]
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

            var facingOrder = new List<Facing>
            {
                Facing.North,
                Facing.South,
                Facing.West,
                Facing.East
            };

            var elfs = new HashSet<Point>(input);
            var round = 0;

            while (true)
            {
                round++;

                var proposedMoves = new Dictionary<Point, Point>();

                //Part 1
                foreach (var elf in elfs)
                {
                    var elfNearby = false;

                    foreach (var neighbor in neighbors)
                    {
                        var p = new Point(elf.x + neighbor.Value.x, elf.y + neighbor.Value.y);

                        if (elfs.Contains(p))
                        {
                            elfNearby = true;

                            break;
                        }
                    }

                    if (elfNearby)
                    {
                        var lastGroup = Facing.North;
                        var foundFacing = false;

                        foreach (var facing in facingOrder)
                        {
                            lastGroup = facing;

                            var group = groups[facing];

                            foreach (var neighbor in group)
                            {
                                var p = new Point(elf.x + neighbor.x, elf.y + neighbor.y);

                                if (elfs.Contains(p))
                                {
                                    goto nextFacing;
                                }
                            }

                            foundFacing = true;
                            break;

                        nextFacing:;
                        }

                        if (foundFacing)
                        {
                            var direction = neighbors[lastGroup];
                            proposedMoves.Add(elf, new Point(elf.x + direction.x, elf.y + direction.y));
                        }
                    }
                }

                if (proposedMoves.Count == 0 || (maxRounds > 0 && round > maxRounds))
                {
                    break;
                }

                //Part 2
                foreach (var proposedMove in proposedMoves)
                {
                    if (proposedMoves.Count(x => x.Value == proposedMove.Value) > 1)
                    {
                        continue;
                    }

                    elfs.Remove(proposedMove.Key);
                    elfs.Add(proposedMove.Value);
                }

                proposedMoves.Clear();

                //Cleanup
                var temp = facingOrder[0];
                facingOrder.RemoveAt(0);
                facingOrder.Add(temp);
            }

            return (elfs, round);
        }

        private static void DrawGrid(HashSet<Point> elfs)
        {
            var stringBuilder = new StringBuilder();

            var minX = elfs.Min(x => x.x);
            var maxX = elfs.Max(x => x.x);
            var minY = elfs.Min(x => x.y);
            var maxY = elfs.Max(x => x.y);

            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    if (elfs.Contains(new Point(x, y)))
                    {
                        stringBuilder.Append('#');
                    }
                    else
                    {
                        stringBuilder.Append('.');
                    }
                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder.ToString());
        }
    }
}