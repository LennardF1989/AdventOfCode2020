using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2020.Days
{
    public static class Day24
    {
        enum Direction
        {
            NorthEast = 0,
            East,
            SouthEast,
            SouthWest,
            West,
            NorthWest
        }

        class Tile
        {
            public int X { get; set; }
            public int Y { get; set; }
            public bool IsBlack { get; set; }

            public override string ToString()
            {
                return IsBlack ? "#" : ".";
            }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day24_Test.txt");
            //var lines = File.ReadAllLines("Content\\Day24_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day24.txt");

            var parsedLines = ParseInput(lines);
            var tiles = FlipTiles(parsedLines);

            int answer = tiles.Values.Count(x => x);

            Logger.Info($"Day 24A: {answer}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day24_Test.txt");
            //var lines = File.ReadAllLines("Content\\Day24_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day24.txt");

            var parsedLines = ParseInput(lines);
            var tiles = FlipTiles(parsedLines);

            var newTiles = tiles.ToDictionary(
                x => x.Key.X + "_" + x.Key.Y,
                x => new Tile {
                    X = (int) x.Key.X,
                    Y = (int) x.Key.Y,
                    IsBlack = x.Value
                }
            );

            //NOTE: Expand the neighbors once
            GetFlipTiles(newTiles);

            int answer = 0;
            for (var i = 0; i < 100; i++)
            {
                FlipTilesAgain(newTiles);

                answer = newTiles.Count(x => x.Value.IsBlack);

                Logger.Debug($"-> Day {i + 1}: {answer}");
            }

            Logger.Info($"Day 24B: {answer}");
        }

        private static void FlipTilesAgain(Dictionary<string, Tile> tiles)
        {
            var flipTiles = GetFlipTiles(tiles);

            foreach (var flipTile in flipTiles)
            {
                flipTile.IsBlack = !flipTile.IsBlack;
            }
        }

        private static List<Tile> GetFlipTiles(Dictionary<string, Tile> tiles)
        {
            var flipTiles = new List<Tile>();

            var tilesToCheck = tiles.Values.ToList();

            foreach (var tile in tilesToCheck)
            {
                var neighbours = GetNeighbours(tiles, tile.X, tile.Y);

                var count = neighbours.Count(s => s.IsBlack);

                if (tile.IsBlack && (count == 0 || count > 2))
                {
                    flipTiles.Add(tile);
                }
                else if (!tile.IsBlack && count == 2)
                {
                    flipTiles.Add(tile);
                }
            }

            return flipTiles;
        }

        private static List<Tile> GetNeighbours(Dictionary<string, Tile> tiles, int x, int y)
        {
            if (!tiles.TryGetValue($"{x + 1}_{y - 1}", out Tile ne))
            {
                ne = new Tile
                {
                    IsBlack = false,
                    X = x + 1,
                    Y = y - 1
                };

                tiles.Add(
                    $"{ne.X}_{ne.Y}", 
                    ne
                );
            }

            if (!tiles.TryGetValue($"{x + 1}_{y}", out Tile e))
            {
                e = new Tile
                {
                    IsBlack = false,
                    X = x + 1,
                    Y = y
                };

                tiles.Add(
                    $"{e.X}_{e.Y}", 
                    e
                );
            }

            if (!tiles.TryGetValue($"{x}_{y + 1}", out Tile se))
            {
                se = new Tile
                {
                    IsBlack = false,
                    X = x,
                    Y = y + 1
                };

                tiles.Add(
                    $"{se.X}_{se.Y}", 
                    se
                );
            }

            if (!tiles.TryGetValue($"{x - 1}_{y + 1}", out Tile sw))
            {
                sw = new Tile
                {
                    IsBlack = false,
                    X = x - 1,
                    Y = y + 1
                };

                tiles.Add(
                    $"{sw.X}_{sw.Y}", 
                    sw
                );
            }

            if (!tiles.TryGetValue($"{x - 1}_{y}", out Tile w))
            {
                w = new Tile
                {
                    IsBlack = false,
                    X = x - 1,
                    Y = y
                };

                tiles.Add(
                    $"{w.X}_{w.Y}", 
                    w
                );
            }

            if (!tiles.TryGetValue($"{x}_{y - 1}", out Tile nw))
            {
                nw = new Tile
                {
                    IsBlack = false,
                    X = x,
                    Y = y - 1
                };

                tiles.Add(
                    $"{nw.X}_{nw.Y}", 
                    nw
                );
            }

            return new List<Tile>
            {
                ne, e, se, sw, w, nw
            };
        }

        private static List<List<Direction>> ParseInput(string[] lines)
        {
            var parsedLines = new List<List<Direction>>();

            foreach (var line in lines)
            {
                var directions = new List<Direction>();

                for (var i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    char c2 = i + 1 < line.Length ? line[i + 1] : '?';

                    switch (c)
                    {
                        case 'e':
                            directions.Add(Direction.East);
                            break;
                        case 'w':
                            directions.Add(Direction.West);
                            break;
                        case 'n' when c2 == 'w':
                            i++;
                            directions.Add(Direction.NorthWest);
                            break;
                        case 'n' when c2 == 'e':
                            i++;
                            directions.Add(Direction.NorthEast);
                            break;
                        case 's' when c2 == 'w':
                            i++;
                            directions.Add(Direction.SouthWest);
                            break;
                        case 's' when c2 == 'e':
                            i++;
                            directions.Add(Direction.SouthEast);
                            break;
                    }
                }

                parsedLines.Add(directions);
            }

            return parsedLines;
        }

        private static Dictionary<Vector2, bool> FlipTiles(List<List<Direction>> parsedLines)
        {
            Dictionary<Vector2, bool> tiles = new Dictionary<Vector2, bool>();

            foreach (var parsedLine in parsedLines)
            {
                var x = 0;
                var y = 0;

                foreach (var direction in parsedLine)
                {
                    if (direction == Direction.NorthEast)
                    {
                        y -= 1;
                        x += 1;
                    }
                    else if (direction == Direction.East)
                    {
                        x += 1;
                    }
                    else if (direction == Direction.SouthEast)
                    {
                        y += 1;
                        //NOTE: Don't touch X
                    }
                    else if (direction == Direction.SouthWest)
                    {
                        y += 1;
                        x -= 1;
                    }
                    else if (direction == Direction.West)
                    {
                        x -= 1;
                    }
                    else if (direction == Direction.NorthWest)
                    {
                        y -= 1;
                        //NOTE: Don't touch X
                    }
                }

                var coord = new Vector2(x, y);
                if (tiles.TryGetValue(coord, out var value))
                {
                    tiles[coord] = !value;
                }
                else
                {
                    tiles.Add(coord, true);
                }
            }

            return tiles;
        }
    }
}
