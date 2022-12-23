using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day22
    {
        enum Face
        {
            Top = 0,
            Front,
            Bottom,
            Left,
            Right,
            Back
        }

        enum State
        {
            Empty = 0,
            Path,
            Wall
        }

        enum EInstruction
        {
            Move,
            TurnLeft,
            TurnRight,
        }

        enum Facing
        {
            North = 0,
            East,
            South,
            West
        }

        class Tile
        {
            public int X { get; set; }
            public int Y { get; set; }
            public State State { get; set; }
            public Tile[] Neighbors { get; set; }

            public Tile(int x, int y, State state)
            {
                X = x;
                Y = y;
                State = state;
                Neighbors = new Tile[4];
            }

            public Tile GetNeighborFor(Facing facing)
            {
                return Neighbors[(int)facing];
            }

            public override string ToString()
            {
                return State == State.Wall ? "#" : ".";
            }
        }

        private sealed record Input(List<List<Tile>> grid, List<Instruction> instructions);
        private sealed record Input2(
            List<List<Tile>> grid,
            Dictionary<Face, List<List<Tile>>> faces,
            Dictionary<Face, Dictionary<Facing, FaceMap>> faceMapping,
            List<Instruction> instructions
        );
        private sealed record Instruction(EInstruction type, int value);
        private sealed record FaceMap(Face Face, int offset);

        public static void StartA()
        {
            var lines = File
                //.ReadAllText("Content//Day22_Test.txt")
                .ReadAllText("Content//Day22.txt")
                .Split("\n\n");

            var result = ParseInput(lines);
            var current = result.grid[0].First(x => x?.State == State.Path);
            var answer = FollowInstructions(result, current);

            Logger.Info($"Day 22A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllText("Content//Day22_Test.txt")
                .ReadAllText("Content//Day22.txt")
                .Split("\n\n");

            var result = ParseInput2(lines);
            var answer = FollowInstructions2(result, 8, 0);

            Logger.Info($"Day 22B: {answer}");
        }

        private static int FollowInstructions(Input result, Tile current)
        {
            var instructions = new Queue<Instruction>(result.instructions);
            var facing = Facing.East;

            while (instructions.Count > 0)
            {
                var instruction = instructions.Dequeue();

                if (instruction.type == EInstruction.Move)
                {
                    for (var i = 0; i < instruction.value; i++)
                    {
                        var newCurrent = current.GetNeighborFor(facing);

                        if (newCurrent == null)
                        {
                            break;
                        }

                        current = newCurrent;
                    }
                }
                else if (instruction.type == EInstruction.TurnLeft)
                {
                    var tempFacing = (int)facing - 1;
                    facing = tempFacing < 0 ? Facing.West : (Facing)tempFacing;
                }
                else if (instruction.type == EInstruction.TurnRight)
                {
                    var tempFacing = (int)facing + 1;
                    facing = tempFacing > 3 ? Facing.North : (Facing)tempFacing;
                }
            }

            var facingValue = facing switch
            {
                Facing.North => 3,
                Facing.East => 0,
                Facing.South => 1,
                Facing.West => 2
            };

            var answer = (current.Y + 1) * 1000 + (current.X + 1) * 4 + facingValue;
            return answer;
        }

        private static int FollowInstructions2(Input2 result, int positionX, int positionY)
        {
            var instructions = new Queue<Instruction>(result.instructions);

            var currentFace = Face.Top;
            var facing = Facing.East;

            //const int size = 4;
            const int size = 50;
            var faceX = positionX % size;
            var faceY = positionY % size;
            var face = result.faces[currentFace];
            var current = result.grid[positionY][positionY];

            var path = new HashSet<(int x, int y)>
            {
                (positionX, positionY)
            };

            DrawGrid(result.grid, path, positionX, positionY);

            void TurnRight(int offset)
            {
                if (offset > 0)
                {
                    var tempFacing = (int)facing + (4 - offset);
                    facing = tempFacing > 3 ? Facing.North : (Facing)tempFacing;
                }

                for (var i2 = 0; i2 < offset; i2++)
                {
                    var tempX = faceX;
                    var tempY = faceY;

                    faceY = size - 1 - tempX;
                    faceX = tempY;
                }
            }

            while (instructions.Count > 0)
            {
                var instruction = instructions.Dequeue();

                if (instruction.type == EInstruction.Move)
                {
                    for (var i = 0; i < instruction.value; i++)
                    {
                        var originalFace = currentFace;
                        var originalFaceX = faceX;
                        var originalFaceY = faceY;

                        var faceNeighbors = result.faceMapping[currentFace];

                        if (facing == Facing.North)
                        {
                            faceY--;

                            if (faceY < 0)
                            {
                                faceY = size - 1;

                                currentFace = faceNeighbors[Facing.North].Face;

                                TurnRight(faceNeighbors[Facing.North].offset);

                                face = result.faces[currentFace];
                            }
                        }
                        else if (facing == Facing.South)
                        {
                            faceY++;

                            if (faceY >= size)
                            {
                                faceY = 0;

                                currentFace = faceNeighbors[Facing.South].Face;

                                TurnRight(faceNeighbors[Facing.South].offset);

                                face = result.faces[currentFace];
                            }
                        }
                        else if (facing == Facing.West)
                        {
                            faceX--;

                            if (faceX < 0)
                            {
                                faceX = size - 1;

                                currentFace = faceNeighbors[Facing.West].Face;

                                TurnRight(faceNeighbors[Facing.West].offset);

                                face = result.faces[currentFace];
                            }
                        }
                        else if (facing == Facing.East)
                        {
                            faceX++;

                            if (faceX >= size)
                            {
                                faceX = 0;

                                currentFace = faceNeighbors[Facing.East].Face;

                                TurnRight(faceNeighbors[Facing.East].offset);

                                face = result.faces[currentFace];
                            }
                        }

                        if (face[faceY][faceX].State == State.Wall)
                        {
                            currentFace = originalFace;
                            faceX = originalFaceX;
                            faceY = originalFaceY;

                            break;
                        }

                        current = face[faceY][faceX];
                        path.Add((current.X, current.Y));

                        //DrawGrid(result.grid, path, current.X, current.Y);
                    }

                    //DrawGrid(result.grid, path, current.X, current.Y);
                }
                else if (instruction.type == EInstruction.TurnLeft)
                {
                    var tempFacing = (int)facing - 1;
                    facing = tempFacing < 0 ? Facing.West : (Facing)tempFacing;
                }
                else if (instruction.type == EInstruction.TurnRight)
                {
                    var tempFacing = (int)facing + 1;
                    facing = tempFacing > 3 ? Facing.North : (Facing)tempFacing;
                }
            }

            DrawGrid(result.grid, path, current.X, current.Y);

            var facingValue = facing switch
            {
                Facing.North => 3,
                Facing.East => 0,
                Facing.South => 1,
                Facing.West => 2
            };

            //Too low: 111049
            //Too low: 111050
            //Too low: 111051
            var answer = (current.Y + 1) * 1000 + (current.X + 1) * 4 + facingValue;

            return answer;
        }

        private static List<Instruction> ParseInstructions(string[] lines)
        {
            var instructions = new List<Instruction>();

            var previousWasDigit = false;
            var digit = 0;
            for (var i = 0; i < lines[1].Length; i++)
            {
                if (char.IsDigit(lines[1][i]))
                {
                    if (previousWasDigit)
                    {
                        digit *= 10;
                        digit += int.Parse(lines[1][i].ToString());
                    }
                    else
                    {
                        digit = int.Parse(lines[1][i].ToString());
                    }

                    previousWasDigit = true;
                }
                else
                {
                    if (previousWasDigit)
                    {
                        instructions.Add(new Instruction(EInstruction.Move, digit));
                        digit = 0;

                        previousWasDigit = false;
                    }

                    instructions.Add(lines[1][i] == 'L'
                        ? new Instruction(EInstruction.TurnLeft, 0)
                        : new Instruction(EInstruction.TurnRight, 0)
                    );
                }
            }

            if (previousWasDigit)
            {
                instructions.Add(new Instruction(EInstruction.Move, digit));
            }

            return instructions;
        }

        private static Input ParseInput(string[] lines)
        {
            var gridLines = lines[0]
                .Split("\n")
                .Select(x => x.ToCharArray())
                .ToList();

            var maxWidth = gridLines.Max(x => x.Length);

            var grid = new List<List<Tile>>();

            for (var y = 0; y < gridLines.Count; y++)
            {
                var gridLine = gridLines[y];

                grid.Add(new List<Tile>());

                for (var x = 0; x < maxWidth; x++)
                {
                    if (x >= gridLine.Length)
                    {
                        grid[y].Add(null);

                        continue;
                    }

                    grid[y].Add(gridLine[x] switch
                    {
                        ' ' => null,
                        '.' => new Tile(x, y, State.Path),
                        '#' => new Tile(x, y, State.Wall),
                        _ => throw new Exception("PANIC!")
                    });
                }
            }

            for (var y = 0; y < grid.Count; y++)
            {
                for (var x = 0; x < grid[y].Count; x++)
                {
                    var gridTile = grid[y][x];

                    if (gridTile == null)
                    {
                        continue;
                    }

                    //North
                    {
                        var north = y;

                        for (var i = 0; i < grid.Count; i++)
                        {
                            north--;

                            if (north < 0)
                            {
                                north = grid.Count - 1;
                            }

                            var foundTile = grid[north][x];

                            if (foundTile == null)
                            {
                                continue;
                            }

                            if (foundTile.State == State.Path)
                            {
                                gridTile.Neighbors[(int)Facing.North] = foundTile;
                            }

                            break;
                        }
                    }

                    //South
                    {
                        var north = y;

                        for (var i = 0; i < grid.Count; i++)
                        {
                            north++;

                            if (north >= grid.Count)
                            {
                                north = 0;
                            }

                            var foundTile = grid[north][x];

                            if (foundTile == null)
                            {
                                continue;
                            }

                            if (foundTile.State == State.Path)
                            {
                                gridTile.Neighbors[(int)Facing.South] = foundTile;
                            }

                            break;
                        }
                    }

                    //West
                    {
                        var west = x;

                        for (var i = 0; i < grid.Count; i++)
                        {
                            west--;

                            if (west < 0)
                            {
                                west = grid[y].Count - 1;
                            }

                            var foundTile = grid[y][west];

                            if (foundTile == null)
                            {
                                continue;
                            }

                            if (foundTile.State == State.Path)
                            {
                                gridTile.Neighbors[(int)Facing.West] = foundTile;
                            }

                            break;
                        }
                    }

                    //East
                    {
                        var west = x;

                        for (var i = 0; i < grid.Count; i++)
                        {
                            west++;

                            if (west >= grid[y].Count)
                            {
                                west = 0;
                            }

                            var foundTile = grid[y][west];

                            if (foundTile == null)
                            {
                                continue;
                            }

                            if (foundTile.State == State.Path)
                            {
                                gridTile.Neighbors[(int)Facing.East] = foundTile;
                            }

                            break;
                        }
                    }
                }
            }

            var instructions = ParseInstructions(lines);

            return new Input(grid, instructions);
        }

        private static Input2 ParseInput2(string[] lines)
        {
            var gridLines = lines[0]
                .Split("\n")
                .Select(x => x.ToCharArray())
                .ToList();

            var maxWidth = gridLines.Max(x => x.Length);

            var grid = new List<List<Tile>>();

            for (var y = 0; y < gridLines.Count; y++)
            {
                var gridLine = gridLines[y];

                grid.Add(new List<Tile>());

                for (var x = 0; x < maxWidth; x++)
                {
                    if (x >= gridLine.Length)
                    {
                        grid[y].Add(null);

                        continue;
                    }

                    grid[y].Add(gridLine[x] switch
                    {
                        ' ' => null,
                        '.' => new Tile(x, y, State.Path),
                        '#' => new Tile(x, y, State.Wall),
                        _ => throw new Exception("PANIC!")
                    });
                }
            }

            //Test
            //var size = 4;
            var size = 50;
            Dictionary<Face, List<List<Tile>>> faces;
            Dictionary<Face, Dictionary<Facing, FaceMap>> faceMapping;
            
            //Input
            //0 T R
            //0 F 0
            //L B 0
            //b 0 0
            {
                var top = ExtractSide(grid, 1, 0, size);
                var front = ExtractSide(grid, 1, 1, size);
                var bottom = ExtractSide(grid, 1, 2, size);
                var left = ExtractSide(grid, 0, 2, size);
                var right = ExtractSide(grid, 2, 0, size);
                var back = ExtractSide(grid, 0, 3, size);

                var topMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Back, 3) },
                    { Facing.East, new FaceMap(Face.Right, 0) },
                    { Facing.South, new FaceMap(Face.Front, 0) },
                    { Facing.West, new FaceMap(Face.Left, 2) }
                };

                var frontMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Top, 0) },
                    { Facing.East, new FaceMap(Face.Right, 1) },
                    { Facing.South, new FaceMap(Face.Bottom, 0) },
                    { Facing.West, new FaceMap(Face.Left, 1) }
                };

                var bottomMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Front, 0) },
                    { Facing.East, new FaceMap(Face.Right, 2) },
                    { Facing.South, new FaceMap(Face.Back, 3) },
                    { Facing.West, new FaceMap(Face.Left, 0) }
                };

                var backMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Left, 0) },
                    { Facing.East, new FaceMap(Face.Bottom, 1) },
                    { Facing.South, new FaceMap(Face.Right, 0) },
                    { Facing.West, new FaceMap(Face.Top, 1) }
                };

                var leftMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Front, 3) },
                    { Facing.East, new FaceMap(Face.Bottom, 0) },
                    { Facing.South, new FaceMap(Face.Back, 0) },
                    { Facing.West, new FaceMap(Face.Top, 2) }
                };

                var rightMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Back, 0) },
                    { Facing.East, new FaceMap(Face.Bottom, 2) },
                    { Facing.South, new FaceMap(Face.Front, 3) },
                    { Facing.West, new FaceMap(Face.Top, 0) }
                };

                faces = new Dictionary<Face, List<List<Tile>>>
                {
                    [Face.Top] = top,
                    [Face.Front] = front,
                    [Face.Bottom] = bottom,
                    [Face.Back] = back,
                    [Face.Left] = left,
                    [Face.Right] = right
                };

                faceMapping = new Dictionary<Face, Dictionary<Facing, FaceMap>>
                {
                    [Face.Top] = topMapping,
                    [Face.Front] = frontMapping,
                    [Face.Bottom] = bottomMapping,
                    [Face.Back] = backMapping,
                    [Face.Left] = leftMapping,
                    [Face.Right] = rightMapping
                };
            }

            //Test
            //0 0 T 0
            //b L F 0
            //0 0 B R
            /*{
                var top = ExtractSide(grid, 2, 0, size);
                var front = ExtractSide(grid, 2, 1, size);
                var bottom = ExtractSide(grid, 2, 2, size);
                var left = ExtractSide(grid, 1, 1, size);
                var right = ExtractSide(grid, 3, 2, size);
                var back = ExtractSide(grid, 0, 1, size);

                var topMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Back, 2) },
                    { Facing.East, new FaceMap(Face.Right, 2) },
                    { Facing.South, new FaceMap(Face.Front, 0) },
                    { Facing.West, new FaceMap(Face.Left, 1) }
                };

                var frontMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Top, 0) },
                    { Facing.East, new FaceMap(Face.Right, 3) },
                    { Facing.South, new FaceMap(Face.Bottom, 0) },
                    { Facing.West, new FaceMap(Face.Left, 0) }
                };

                var bottomMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Front, 0) },
                    { Facing.East, new FaceMap(Face.Right, 0) },
                    { Facing.South, new FaceMap(Face.Back, 2) },
                    { Facing.West, new FaceMap(Face.Left, 3) }
                };

                var backMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Top, 2) },
                    { Facing.East, new FaceMap(Face.Left, 0) },
                    { Facing.South, new FaceMap(Face.Bottom, 2) },
                    { Facing.West, new FaceMap(Face.Right, 3) }
                };

                var leftMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Top, 3) },
                    { Facing.East, new FaceMap(Face.Front, 0) },
                    { Facing.South, new FaceMap(Face.Bottom, 1) },
                    { Facing.West, new FaceMap(Face.Back, 0) }
                };

                var rightMapping = new Dictionary<Facing, FaceMap>
                {
                    { Facing.North, new FaceMap(Face.Front, 1) },
                    { Facing.East, new FaceMap(Face.Top, 2) },
                    { Facing.South, new FaceMap(Face.Back, 1) },
                    { Facing.West, new FaceMap(Face.Bottom, 0) }
                };

                faces = new Dictionary<Face, List<List<Tile>>>
                {
                    [Face.Top] = top,
                    [Face.Front] = front,
                    [Face.Bottom] = bottom,
                    [Face.Back] = back,
                    [Face.Left] = left,
                    [Face.Right] = right
                };

                faceMapping = new Dictionary<Face, Dictionary<Facing, FaceMap>>
                {
                    [Face.Top] = topMapping,
                    [Face.Front] = frontMapping,
                    [Face.Bottom] = bottomMapping,
                    [Face.Back] = backMapping,
                    [Face.Left] = leftMapping,
                    [Face.Right] = rightMapping
                };
            }*/

            var instructions = ParseInstructions(lines);

            return new Input2(grid, faces, faceMapping, instructions);
        }

        private static List<List<Tile>> ExtractSide(List<List<Tile>> grid, int x, int y, int size)
        {
            return grid
                .Skip(y * size)
                .Take(size)
                .Select(z => z
                    .Skip(size * x)
                    .Take(size)
                    .ToList()
                )
                .ToList();
        }

        private static void DrawGrid(List<List<Tile>> grid, HashSet<(int x, int y)> path, int currentX, int currentY)
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < grid.Count; y++)
            {
                for (var x = 0; x < grid[y].Count; x++)
                {
                    if (y == currentY && x == currentX)
                    {
                        stringBuilder.Append('x');

                        continue;
                    }

                    if (path.Contains((x, y)))
                    {
                        stringBuilder.Append('o');

                        continue;
                    }

                    switch (grid[y][x]?.State)
                    {
                        case State.Path:
                            stringBuilder.Append('.');
                            break;

                        case State.Wall:
                            stringBuilder.Append('#');
                            break;

                        case null:
                            stringBuilder.Append(' ');
                            break;
                    }

                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder.ToString());
        }
    }
}