using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                return Neighbors[(int) facing];
            }

            public override string ToString()
            {
                return State == State.Wall ? "#" : ".";
            }
        }

        private sealed record Input(List<List<Tile>> grid, List<Instruction> instructions);

        private sealed record Instruction(EInstruction type, int value);

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
                .ReadAllText("Content//Day22_Test.txt")
                //.ReadAllText("Content//Day22.txt")
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
                    var tempFacing = (int) facing - 1;
                    facing = tempFacing < 0 ? Facing.West : (Facing) tempFacing;
                }
                else if (instruction.type == EInstruction.TurnRight)
                {
                    var tempFacing = (int) facing + 1;
                    facing = tempFacing > 3 ? Facing.North : (Facing) tempFacing;
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

        private static int FollowInstructions2(Input result, int positionX, int positionY)
        {
            var instructions = new Queue<Instruction>(result.instructions);
            var facing = Facing.East;

            var path = new HashSet<(int x, int y)>
            {
                (positionX, positionY)
            };

            DrawGrid(result.grid, path, positionX, positionY);

            while (instructions.Count > 0)
            {
                var instruction = instructions.Dequeue();

                if (instruction.type == EInstruction.Move)
                {
                    for (var i = 0; i < instruction.value; i++)
                    {
                        var originalX = positionX;
                        var originalY = positionY;

                        if (facing == Facing.North)
                        {
                            positionY--;

                            if (positionY < 0)
                            {
                                positionY = result.grid.Count - 1;
                            }
                        }
                        else if (facing == Facing.South)
                        {
                            positionY++;

                            if (positionY >= result.grid.Count)
                            {
                                positionY = 0;
                            }
                        }
                        else if (facing == Facing.West)
                        {
                            positionX--;

                            if (positionX < 0)
                            {
                                positionX = result.grid.Count - 1;
                            }
                        }
                        else if (facing == Facing.East)
                        {
                            positionX++;

                            if (positionX >= result.grid.Count)
                            {
                                positionX = 0;
                            }
                        }

                        if (result.grid[positionY][positionX].State == State.Wall)
                        {
                            positionX = originalX;
                            positionY = originalY;

                            var current1 = result.grid[positionY][positionX];
                            path.Add((current1.X, current1.Y));

                            break;
                        }

                        var current = result.grid[positionY][positionX];
                        path.Add((current.X, current.Y));
                    }

                    DrawGrid(result.grid, path, positionX, positionY);
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

            var tile = result.grid[positionY][positionX];

            var answer = (tile.Y + 1) * 1000 + (tile.X + 1) * 4 + facingValue;

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
                                gridTile.Neighbors[(int) Facing.North] = foundTile;
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
                                gridTile.Neighbors[(int) Facing.South] = foundTile;
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
                                gridTile.Neighbors[(int) Facing.West] = foundTile;
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
                                gridTile.Neighbors[(int) Facing.East] = foundTile;
                            }

                            break;
                        }
                    }
                }
            }

            var instructions = ParseInstructions(lines);

            return new Input(grid, instructions);
        }

        private static Input ParseInput2(string[] lines)
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
            var size = 4;
            var sides = new List<List<List<Tile>>>();

            //Test
            //0 0 T 0
            //b L F 0
            //0 0 B R
            {
                var top = ExtractSide(grid, 2, 0, size);
                var front = ExtractSide(grid, 2, 1, size);
                var bottom = ExtractSide(grid, 2, 2, size);
                var left = ExtractSide(grid, 1, 1, size);
                var right = ExtractSide(grid, 3, 2, size);
                var back = ExtractSide(grid, 0, 1, size);

                var topMapping = new Dictionary<Facing, List<List<Tile>>>
                {
                    { Facing.North, RotateSide(back, 180) },
                    { Facing.East, RotateSide(right, 180) },
                    { Facing.South, front },
                    { Facing.West, RotateSide(left, 90)}
                };

                var frontMapping = new Dictionary<Facing, List<List<Tile>>>
                {
                    { Facing.North, top },
                    { Facing.East, RotateSide(right, 270) },
                    { Facing.South, bottom },
                    { Facing.West, left }
                };

                var bottomMapping = new Dictionary<Facing, List<List<Tile>>>
                {
                    { Facing.North, front },
                    { Facing.East, right },
                    { Facing.South, RotateSide(back, 180) },
                    { Facing.West, RotateSide(left, 270) }
                };

                var backMapping = new Dictionary<Facing, List<List<Tile>>>
                {
                    { Facing.North, RotateSide(top, 180) },
                    { Facing.East, left },
                    { Facing.South, RotateSide(bottom, 180) },
                    { Facing.West, RotateSide(right, 270) }
                };

                var leftMapping = new Dictionary<Facing, List<List<Tile>>>
                {
                    { Facing.North, RotateSide(top, 270) },
                    { Facing.East, front },
                    { Facing.South, RotateSide(bottom, 90) },
                    { Facing.West, back }
                };

                var rightMapping = new Dictionary<Facing, List<List<Tile>>>
                {
                    { Facing.North, RotateSide(front, 90) },
                    { Facing.East, RotateSide(top, 180) },
                    { Facing.South, RotateSide(back, 90) },
                    { Facing.West, bottom }
                };

                var faceMapping = new Dictionary<Face, Dictionary<Facing, List<List<Tile>>>>();
                faceMapping[Face.Top] = topMapping;
                faceMapping[Face.Front] = frontMapping;
                faceMapping[Face.Bottom] = bottomMapping;
                faceMapping[Face.Back] = backMapping;
                faceMapping[Face.Left] = leftMapping;
                faceMapping[Face.Right] = rightMapping;

                /*//01 02 _T 04
                //_b _L _F 08
                //09 10 _B _R
                //13 14 15 16

                //01 = b (180)
                var s01 = RotateSide(back, 180);

                //02 = L(90)
                var s02 = RotateSide(left, 90);

                //T = -
                var s03 = top;

                //04 = R(180)
                var s04 = RotateSide(right, 180);

                //b = -
                var s05 = back;

                //L = -
                var s06 = left;

                //F = -
                var s07 = front;

                //08 = R(270)
                var s08 = RotateSide(right, 270);

                //09 = T(180)
                var s09 = RotateSide(top, 180);

                //10 = L(270)
                var s10 = RotateSide(left, 270);

                //B = -
                var s11 = bottom;

                //R = -
                var s12 = right;

                //13 = F(180)
                var s13 = RotateSide(front, 180);

                //14 = L(180)
                var s14 = RotateSide(left, 180);

                //15 = b(180)
                var s15 = RotateSide(back, 180);

                //16 = R(90)
                var s16 = RotateSide(right, 90);

                sides = new List<List<List<Tile>>>
                {
                    s01, s02, s03, s04,
                    s05, s06, s07, s08,
                    s09, s10, s11, s12,
                    s13, s14, s15, s16
                };

                StitchCube(grid, sides, 4);*/
            }

            //Input
            //0 T R
            //0 F 0
            //L B 0
            //b 0 0

            /*for (var y = 0; y < grid.Count; y++)
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
            }*/

            var instructions = ParseInstructions(lines);

            return new Input(grid, instructions);
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

        private static void StitchCube(List<List<Tile>> grid, List<List<List<Tile>>> sides, int size)
        {
            for (int i = 0; i < size; i++)
            {
                grid.Add(new List<Tile>(Enumerable.Repeat((Tile)null, size * 4)));
            }

            for (int i = 0; i < 4; i++)
            {
                var listOffset = i * size;

                for (var y = 0; y < size; y++)
                {
                    grid[listOffset + y] = sides[listOffset + 0][y]
                        .Union(sides[listOffset + 1][y])
                        .Union(sides[listOffset + 2][y])
                        .Union(sides[listOffset + 3][y])
                        .ToList();
                }
            }
        }

        private static List<List<T>> RotateSide<T>(List<List<T>> side, int degrees)
        {
            var copy = side;

            for (var i = 0; i < degrees / 90; i++)
            {
                copy = RotateSide90(copy);
            }

            return copy;
        }

        private static List<List<T>> RotateSide90<T>(List<List<T>> side)
        {
            var copy = side
                .Select(t => Enumerable.Repeat(default(T), side.Count).ToList())
                .ToList();

            for (var y = 0; y < side.Count; y++)
            {
                for (var x = 0; x < side.Count; x++)
                {
                    copy[y][x] = side[side.Count - x - 1][y];
                }
            }

            return copy;
        }

        private static void DrawGrid(List<List<Tile>> grid, HashSet<(int x, int y)> path, int currentX, int currentY)
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < grid.Count; y++)
            {
                for (var x = 0; x < grid.Count; x++)
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

                    switch (grid[y][x].State)
                    {
                        case State.Path:
                            stringBuilder.Append('.');
                            break;

                        case State.Wall:
                            stringBuilder.Append('#');
                            break;
                    }

                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder.ToString());
        }
    }
}