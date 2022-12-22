using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day22
    {
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
            var answer = FollowInstructions(result);

            Logger.Info($"Day 22A: {answer}");
        }
        
        public static void StartB()
        {
            var lines = File
                .ReadAllText("Content//Day22_Test.txt")
                //.ReadAllText("Content//Day22.txt")
                .Split("\n\n");

            var result = ParseInput2(lines);
            var answer = FollowInstructions(result);

            Logger.Info($"Day 22B: {answer}");
        }

        private static int FollowInstructions(Input result)
        {
            var instructions = new Queue<Instruction>(result.instructions);
            var facing = Facing.East;
            var current = result.grid[0].First(x => x?.State == State.Path);

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

            var grid = new List<List<Tile>>();

            var instructions = ParseInstructions(lines);

            return new Input(grid, instructions);
        }
    }
}