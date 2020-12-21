using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Days
{
    public static class Day20
    {
        private const int SIZE = 10;

        enum Tile
        {
            Enabled = 1,
            Disabled = 0,
            Seamonster = 2
        }

        enum Direction
        {
            North = 0,
            East,
            South,
            West
        }

        class Grid
        {
            public int ID { get; set; }
            public List<Tile> Tiles { get; set; }
            public int Size { get; }

            public List<Grid> AllOrientations { get; private set; }

            public Grid(int size)
            {
                Size = size;
            }

            public void GenerateAllOrientations()
            {
                var grids = new List<Grid>();

                void RotateThreeTimes(Grid lastGrid)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var rotateGrid = lastGrid.Clone();
                        rotateGrid.Rotate90();

                        grids.Add(rotateGrid);

                        lastGrid = rotateGrid;
                    }
                }

                //Default
                var grid = Clone();
                grids.Add(grid);
                RotateThreeTimes(grid);

                //Horizontal
                grid = Clone();
                grid.FlipHorizontal();
                grids.Add(grid);
                RotateThreeTimes(grid);

                //Vertical
                grid = Clone();
                grid.FlipVertical();
                grids.Add(grid);
                RotateThreeTimes(grid);

                AllOrientations = grids;
            }

            public string[] GetEdges()
            {
                string north = GetEdge(Direction.North);
                string east = GetEdge(Direction.East);
                string south = GetEdge(Direction.South);
                string west = GetEdge(Direction.West);

                return new[] { north, east, south, west };
            }

            public string GetEdge(Direction direction)
            {
                switch (direction)
                {
                    case Direction.North:
                        return string.Join(string.Empty, Tiles.Skip(0).Take(Size).Select(x => (int)x));
                    case Direction.East:
                        return string.Join(string.Empty, Tiles.Where((x, i) => (i + 1) % Size == 0).Select(x => (int)x));
                    case Direction.South:
                        return string.Join(string.Empty, Tiles.Skip(Size * Size - Size).Take(Size).Select(x => (int)x));
                    case Direction.West:
                        return string.Join(string.Empty, Tiles.Where((x, i) => (i + 1) % Size == 1).Select(x => (int)x));
                }

                return null;
            }

            private void Rotate90()
            {
                var tiles = new List<Tile>(Tiles);

                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        tiles[GetIndex(x, y)] = GetTile(y, Size - x - 1);
                    }
                }

                Tiles = tiles;
            }

            private void FlipHorizontal()
            {
                var tiles = new List<Tile>(Tiles);

                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        tiles[GetIndex(x, y)] = GetTile(x, Size - y - 1);
                    }
                }

                Tiles = tiles;
            }

            private void FlipVertical()
            {
                var tiles = new List<Tile>(Tiles);

                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        tiles[GetIndex(x, y)] = GetTile(Size - x - 1, y);
                    }
                }

                Tiles = tiles;
            }

            public List<Tile> GetLine(int y)
            {
                return Tiles
                    .Skip(y * Size)
                    .Take(Size)
                    .ToList();
            }

            private int GetIndex(int x, int y)
            {
                return y * Size + x;
            }

            public Tile GetTile(int x, int y)
            {
                return Tiles[GetIndex(x, y)];
            }

            public void SetTile(int x, int y, Tile tile)
            {
                Tiles[GetIndex(x, y)] = tile;
            }

            private Grid Clone()
            {
                return new Grid(Size)
                {
                    ID = ID,
                    Tiles = new List<Tile>(Tiles)
                };
            }

            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"Tile {ID}");

                for (var i = 0; i < Tiles.Count; i++)
                {
                    var tile = Tiles[i];

                    if (tile == Tile.Enabled)
                    {
                        stringBuilder.Append('#');
                    }
                    else if (tile == Tile.Disabled)
                    {
                        stringBuilder.Append('.');
                    }
                    else
                    {
                        stringBuilder.Append('O');
                    }

                    if ((i + 1) % Size == 0)
                    {
                        stringBuilder.AppendLine();
                    }
                }

                return stringBuilder.ToString();
            }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day20_Test.txt");
            var lines = File.ReadAllLines("Content\\Day20.txt");

            var grids = GetGrids(lines);
            var gridMapping = GetGridMapping(grids);

            var result = gridMapping
                .Where(x => x.Value[Direction.North].Count == 0 && x.Value[Direction.West].Count == 0)
                .Select(x => x.Key.ID)
                .Distinct()
                .ToList();

            Logger.Debug(string.Join(" ", result));

            long answer = result[0];

            for (int i = 1; i < result.Count; i++)
            {
                answer *= result[i];
            }

            Logger.Info($"Day 20A: {answer}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day20_Test.txt");
            var lines = File.ReadAllLines("Content\\Day20.txt");

            var grids = GetGrids(lines);
            var gridMapping = GetGridMapping(grids);

            var imageSize = (int)Math.Sqrt(grids.Count);

            var constructedImage = ConstructImage(gridMapping, imageSize);

            var combinedGrid = CombineGrids(constructedImage, imageSize);

            var answer = SearchSeamonster(combinedGrid);

            Logger.Info($"Day 20B: {answer}");
        }

        private static List<Grid> GetGrids(string[] lines)
        {
            var grids = new List<Grid>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("Tile"))
                {
                    var grid = new Grid(SIZE)
                    {
                        ID = int.Parse(line.Split(" ")[1].Substring(0, 4)),
                        Tiles = lines
                            .Skip(i + 1)
                            .Take(SIZE)
                            .SelectMany(x => x.Select(y => y == '#' ? Tile.Enabled : Tile.Disabled))
                            .ToList()
                    };

                    grid.GenerateAllOrientations();

                    grids.Add(grid);

                    i += 11;
                }
            }

            return grids;
        }

        private static Dictionary<Grid, Dictionary<Direction, List<Grid>>> GetGridMapping(List<Grid> grids)
        {
            var allPossibleGrids = grids
                .SelectMany(x => x.AllOrientations)
                .ToList();

            var gridMapping = new Dictionary<Grid, Dictionary<Direction, List<Grid>>>();

            foreach (var grid in allPossibleGrids)
            {
                var edges = grid.GetEdges();

                //Logger.Debug(grid);

                //North -> South
                var linkedNorthGrids = allPossibleGrids
                    .Where(x => x.ID != grid.ID && edges[0] == x.GetEdge(Direction.South))
                    .ToList();

                //East -> West
                var linkedEastGrids = allPossibleGrids
                    .Where(x => x.ID != grid.ID && edges[1] == x.GetEdge(Direction.West))
                    .ToList();

                //South -> North
                var linkedSouthGrids = allPossibleGrids
                    .Where(x => x.ID != grid.ID && edges[2] == x.GetEdge(Direction.North))
                    .ToList();

                //West -> East
                var linkedWestGrids = allPossibleGrids
                    .Where(x => x.ID != grid.ID && edges[3] == x.GetEdge(Direction.East))
                    .ToList();

                gridMapping.Add(grid, new Dictionary<Direction, List<Grid>>
                {
                    {Direction.North, linkedNorthGrids},
                    {Direction.East, linkedEastGrids},
                    {Direction.South, linkedSouthGrids},
                    {Direction.West, linkedWestGrids},
                });
            }

            return gridMapping;
        }

        private static Grid[,] ConstructImage(Dictionary<Grid, Dictionary<Direction, List<Grid>>> gridMapping, int imageSize)
        {
            var corners = gridMapping
                .Where(x => x.Value[Direction.North].Count == 0 && x.Value[Direction.West].Count == 0)
                .ToList();

            Grid[,] constructedImage = new Grid[imageSize, imageSize];

            var topLeft = corners.First();
            corners.RemoveAll(x => x.Key.ID == topLeft.Key.ID);
            var top = ConstructImageBorder(topLeft.Key, corners, gridMapping, Direction.East, Direction.South, Direction.North);

            for (var i = 0; i < top.Count; i++)
            {
                constructedImage[i, 0] = top[i];
            }

            var topRight = top[imageSize - 1];
            corners.RemoveAll(x => x.Key.ID == topRight.ID);
            var right = ConstructImageBorder(topRight, corners, gridMapping, Direction.South, Direction.West, Direction.East);

            for (var i = 0; i < right.Count; i++)
            {
                constructedImage[imageSize - 1, i] = right[i];
            }

            var bottomRight = right[imageSize - 1];
            corners.RemoveAll(x => x.Key.ID == bottomRight.ID);
            var bottom = ConstructImageBorder(bottomRight, corners, gridMapping, Direction.West, Direction.North,
                Direction.South);

            for (var i = 0; i < bottom.Count; i++)
            {
                constructedImage[imageSize - 1 - i, imageSize - 1] = bottom[i];
            }

            var bottomLeft = bottom[imageSize - 1];
            corners.RemoveAll(x => x.Key.ID == bottomLeft.ID);
            corners.Add(topLeft);
            var left = ConstructImageBorder(bottomLeft, corners, gridMapping, Direction.North, Direction.East, Direction.West);

            for (var i = 0; i < left.Count; i++)
            {
                constructedImage[0, imageSize - 1 - i] = left[i];
            }

            for (int y = 1; y < imageSize - 1; y++)
            {
                for (int x = 1; x < imageSize - 1; x++)
                {
                    var north = constructedImage[x, y - 1];
                    var east = constructedImage[x + 1, y];
                    var south = constructedImage[x, y + 1];
                    var west = constructedImage[x - 1, y];

                    var result = gridMapping.Where(z =>
                        (north == null || z.Value[Direction.North].Contains(north)) &&
                        (east == null || z.Value[Direction.East].Contains(east)) &&
                        (south == null || z.Value[Direction.South].Contains(south)) &&
                        (west == null || z.Value[Direction.West].Contains(west))
                    ).ToList();

                    if (result.Count > 0)
                    {
                        constructedImage[x, y] = result.First().Key;
                    }
                    else
                    {
                        Logger.Debug("No image found!");
                    }
                }
            }

            return constructedImage;
        }

        private static List<Grid> ConstructImageBorder(
            Grid startGrid,
            List<KeyValuePair<Grid, Dictionary<Direction, List<Grid>>>> corners,
            Dictionary<Grid, Dictionary<Direction, List<Grid>>> gridMapping,
            Direction direction,
            Direction directionNeeds,
            Direction directionShouldnt)
        {
            var gridStack = new Stack<Grid>();
            var gridVisited = new List<Grid>();
            var gridParents = new Dictionary<Grid, Grid>();

            gridStack.Push(startGrid);
            gridParents[startGrid] = null;

            while (gridStack.Count > 0)
            {
                var tempGrid = gridStack.Pop();

                if (corners.Any(x => x.Key.ID == tempGrid.ID))
                {
                    var result = new List<Grid>();

                    var current = tempGrid;
                    while (current != null)
                    {
                        result.Add(current);
                        current = gridParents[current];
                    }

                    result.Reverse();

                    return result;
                }

                if (gridVisited.Contains(tempGrid))
                {
                    continue;
                }

                gridVisited.Add(tempGrid);

                var possibleConnections = gridMapping[tempGrid][direction]
                    .Select(x => gridMapping.First(y => y.Key == x))
                    .Where(x =>
                        x.Value[directionShouldnt].Count == 0 &&
                        x.Value[directionNeeds].Count > 0)
                    .ToList();

                foreach (var possibleConnection in possibleConnections)
                {
                    gridStack.Push(possibleConnection.Key);
                    gridParents[possibleConnection.Key] = tempGrid;
                }
            }

            return null;
        }

        private static Grid CombineGrids(Grid[,] constructedImage, int imageSize)
        {
            var shrinkedSize = SIZE - 2;
            var combinedGrid = new Grid(shrinkedSize * imageSize)
            {
                Tiles = new List<Tile>(imageSize * shrinkedSize * shrinkedSize)
            };

            for (int y = 0; y < imageSize; y++)
            {
                for (int lineY = 1; lineY < SIZE - 1; lineY++)
                {
                    for (int x = 0; x < imageSize; x++)
                    {
                        var grid = constructedImage[x, y];
                        combinedGrid.Tiles.AddRange(grid.GetLine(lineY).Skip(1).Take(shrinkedSize));
                    }
                }
            }

            combinedGrid.GenerateAllOrientations();

            return combinedGrid;
        }

        private static int SearchSeamonster(Grid combinedGrid)
        {
            var seamonsterMask = File
                .ReadAllLines("Content\\Day20_Seamonster.txt")
                .Select(x => x
                    .Select(y => y == '#' ? Tile.Enabled : Tile.Disabled)
                    .ToList()
                )
                .ToList();

            foreach (var orientation in combinedGrid.AllOrientations)
            {
                int enabledBefore = orientation.Tiles.Count(x => x == Tile.Enabled);

                for (int y = 0; y < orientation.Size - seamonsterMask.Count; y++)
                {
                    for (int x = 0; x < orientation.Size - seamonsterMask[0].Count; x++)
                    {
                        bool valid = true;

                        for (int smY = 0; smY < seamonsterMask.Count; smY++)
                        {
                            for (int smX = 0; smX < seamonsterMask[0].Count; smX++)
                            {
                                if (seamonsterMask[smY][smX] == Tile.Disabled)
                                {
                                    continue;
                                }

                                if (orientation.GetTile(x + smX, y + smY) == Tile.Disabled)
                                {
                                    valid = false;
                                }
                            }
                        }

                        if (valid)
                        {
                            for (int smY = 0; smY < seamonsterMask.Count; smY++)
                            {
                                for (int smX = 0; smX < seamonsterMask[0].Count; smX++)
                                {
                                    if (seamonsterMask[smY][smX] == Tile.Disabled)
                                    {
                                        continue;
                                    }

                                    orientation.SetTile(x + smX, y + smY, Tile.Seamonster);
                                }
                            }
                        }
                    }
                }

                int enabledAfter = orientation.Tiles.Count(x => x == Tile.Enabled);

                if (enabledAfter < enabledBefore)
                {
                    return enabledAfter;
                }
            }

            return 0;
        }
    }
}
