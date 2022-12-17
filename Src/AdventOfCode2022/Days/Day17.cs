using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day17
    {
        enum State
        {
            Empty,
            Rock
        }

        class Shape
        {
            public int MinX { get; set; }
            public int MinY { get; set; }
            public int MaxX { get; set; }
            public int MaxY { get; set; }

            public int Width => MaxX - MinX;
            public int Height => MaxY - MinY;

            public State[,] Grid { get; set; }
        }

        class ShapeLineHorizontal : Shape
        {
            public ShapeLineHorizontal()
            {
                MinX = 0;
                MinY = 0;
                MaxX = 4;
                MaxY = 1;

                Grid = new State[MaxY, MaxX];

                Grid[0, 0] = State.Rock;
                Grid[0, 1] = State.Rock;
                Grid[0, 2] = State.Rock;
                Grid[0, 3] = State.Rock;
            }
        }

        class ShapePlus : Shape
        {
            public ShapePlus()
            {
                MinX = 0;
                MinY = 0;
                MaxX = 3;
                MaxY = 3;

                Grid = new State[MaxY, MaxX];

                Grid[0, 1] = State.Rock;
                Grid[1, 0] = State.Rock;
                Grid[1, 1] = State.Rock;
                Grid[1, 2] = State.Rock;
                Grid[2, 1] = State.Rock;
            }
        }

        class ShapeCorner : Shape
        {
            public ShapeCorner()
            {
                MinX = 0;
                MinY = 0;
                MaxX = 3;
                MaxY = 3;

                Grid = new State[MaxY, MaxX];

                Grid[0, 2] = State.Rock;
                Grid[1, 2] = State.Rock;
                Grid[2, 0] = State.Rock;
                Grid[2, 1] = State.Rock;
                Grid[2, 2] = State.Rock;
            }
        }

        class ShapeLineVertical : Shape
        {
            public ShapeLineVertical()
            {
                MinX = 0;
                MinY = 0;
                MaxX = 1;
                MaxY = 4;

                Grid = new State[MaxY, MaxX];

                Grid[0, 0] = State.Rock;
                Grid[1, 0] = State.Rock;
                Grid[2, 0] = State.Rock;
                Grid[3, 0] = State.Rock;
            }
        }

        class ShapeBox : Shape
        {
            public ShapeBox()
            {
                MinX = 0;
                MinY = 0;
                MaxX = 2;
                MaxY = 2;

                Grid = new State[MaxY, MaxX];

                Grid[0, 0] = State.Rock;
                Grid[0, 1] = State.Rock;
                Grid[1, 0] = State.Rock;
                Grid[1, 1] = State.Rock;
            }
        }
        
        class PlacedShape
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Shape Shape { get; set; }

            public int Left => X + Shape.MinX;
            public int Right => X + Shape.MaxX;
            public int Top => Y + Shape.MinY;
            public int Down => Y + Shape.MaxY;
        }

        //TODO: Revert direction for optimization
        class Tetris
        {
            public List<List<State>> Grid { get; }
            public int LastCollisionYIndex { get; private set; }
            public int CurrentShapeIndex { get; private set; }

            private readonly List<Shape> _shapes = new()
            {
                new ShapeLineHorizontal(),
                new ShapePlus(),
                new ShapeCorner(),
                new ShapeLineVertical(),
                new ShapeBox()
            };

            private readonly int _minHeight;
            private readonly int _maxWidth;

            private PlacedShape _currentShape;

            public Tetris(int heightBeforeBottom, int width)
            {
                Grid = new List<List<State>>();

                _minHeight = heightBeforeBottom;
                _maxWidth = width;

                AddRows(heightBeforeBottom);

                LastCollisionYIndex = Grid.Count;
            }

            private void AddRows(int amount)
            {
                for (var i = 0; i < amount; i++)
                {
                    Grid.Insert(0, new List<State>(Enumerable.Repeat(State.Empty, _maxWidth)));
                }
            }

            public void NextShape()
            {
                var shapeIndex = CurrentShapeIndex % _shapes.Count;
                var shape = _shapes[shapeIndex];
                CurrentShapeIndex = shapeIndex + 1;

                var offset = LastCollisionYIndex - _minHeight - shape.Height;

                if (offset < 0)
                {
                    AddRows(Math.Abs(offset));

                    for (var i = 0; i < Math.Abs(offset); i++)
                    {
                        LastCollisionYIndex++;
                    }
                }

                _currentShape = new PlacedShape
                {
                    X = 2,
                    Y = (offset < 0) ? 0 : offset,
                    Shape = shape
                };
            }

            public bool CanMoveToLeft()
            {
                return _currentShape.X > 0 && !CheckForCollisionLeftRight(-1);
            }

            public void MoveLeft()
            {
                _currentShape.X--;
            }

            public void MoveRight()
            {
                _currentShape.X++;
            }

            public void MoveDown()
            {
                _currentShape.Y++;
            }

            public bool CanMoveToRight()
            {
                return _currentShape.X < _currentShape.Right && !CheckForCollisionLeftRight(1);
            }

            public bool CanMoveDown()
            {
                return _currentShape.Y < Grid.Count && !CheckForCollisionDown();
            }

            private bool CheckForCollisionLeftRight(int direction)
            {
                for (var y = 0; y < _currentShape.Shape.Height; y++)
                {
                    for (var x = 0; x < _currentShape.Shape.Width; x++)
                    {
                        var targetX = _currentShape.X + x + direction;

                        if (targetX < 0 || targetX >= _maxWidth)
                        {
                            return true;
                        }

                        if (
                            _currentShape.Shape.Grid[y, x] == State.Rock && 
                            Grid[_currentShape.Y + y][_currentShape.X + x + direction] == State.Rock)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            private bool CheckForCollisionDown()
            {
                for (var y = 0; y < _currentShape.Shape.Height; y++)
                {
                    for (var x = 0; x < _currentShape.Shape.Width; x++)
                    {
                        var targetY = _currentShape.Y + y + 1;

                        if (targetY >= Grid.Count)
                        {
                            return true;
                        }

                        if (
                            _currentShape.Shape.Grid[y, x] == State.Rock &&
                            Grid[_currentShape.Y + y + 1][_currentShape.X + x] == State.Rock)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            public void PlaceShape()
            {
                LastCollisionYIndex = Math.Min(LastCollisionYIndex, _currentShape.Y);

                for (var y = 0; y < _currentShape.Shape.Height; y++)
                {
                    for (var x = 0; x < _currentShape.Shape.Width; x++)
                    {
                        if (_currentShape.Shape.Grid[y, x] == State.Rock)
                        {
                            Grid[_currentShape.Y + y][_currentShape.X + x] = State.Rock;
                        }
                    }
                }
            }

            public void DrawField(bool includeShape)
            {
                var stringBuilder = new StringBuilder();

                for (var y = 0; y < Grid.Count; y++)
                {
                    stringBuilder.Append("|");

                    for (var x = 0; x < _maxWidth; x++)
                    {
                        if (
                            includeShape &&
                            y >= _currentShape.Top && y < _currentShape.Down &&
                            x >= _currentShape.Left && x < _currentShape.Right
                        )
                        {
                            stringBuilder.Append(
                                _currentShape.Shape.Grid[
                                    y - _currentShape.Y,
                                    x - _currentShape.X
                                ] == State.Empty
                                    ? "." 
                                    : "@"
                                );
                        }
                        else
                        {
                            stringBuilder.Append(Grid[y][x] == State.Empty ? "." : "#");
                        }
                    }

                    stringBuilder.AppendLine("|");
                }

                stringBuilder.AppendLine("+-------+");

                Logger.Debug(stringBuilder.ToString());
            }
        }

        public static void StartA()
        {
            var lines = File
                //.ReadAllText("Content\\Day17_Test.txt")
                .ReadAllText("Content\\Day17.txt")
                .ToCharArray()
                ;

            var answer = PlayTetris(lines, 2022);

            Logger.Info($"Day 17A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllText("Content\\Day17_Test.txt")
                .ReadAllText("Content\\Day17.txt")
                .ToCharArray()
                ;

            var answer = PlayTetris(lines, 1_000_000_000_000);

            Logger.Info($"Day 17B: {answer}");
        }

        private static long PlayTetris(char[] lines, long rocks)
        {
            var tetris = new Tetris(3, 7);

            var instructions = new Queue<char>(lines);
            var instructionCount = 0;

            //NOTE: A line can change over a few iterations, so we need to sample a bunch of lines in a row
            const int minimumPatternSize = 20;

            var enablePatterns = true;
            var patterns = new HashSet<string>();
            var patternCount = 0;
            var patternToFind = string.Empty;

            var oldShapeCount = 0L;
            var oldHeight = 0;
            var extraHeight = 0L;

            var shapeCount = 0L;

            while (shapeCount < rocks)
            {
                //Logger.Debug("Start");

                tetris.NextShape();

                //tetris.DrawField(true);

                shapeCount++;

                //NOTE: Start tracking patterns after the first rock has fallen
                if (enablePatterns && shapeCount > 1)
                {
                    var state = string.Join(
                        string.Empty,
                        tetris.Grid[tetris.LastCollisionYIndex].Select(x => x == State.Empty ? "." : "#")
                    );

                    var key = $"{tetris.CurrentShapeIndex - 1}_{instructionCount}_{state}";

                    if (patterns.Contains(key))
                    {
                        patternCount++;

                        if (patternToFind == string.Empty && patternCount > minimumPatternSize)
                        {
                            patternToFind = key;
                            oldHeight = tetris.Grid.Count;
                            oldShapeCount = shapeCount;
                        }
                        else if (patternToFind == key)
                        {
                            var currentHeight = tetris.Grid.Count;
                            var heightDifference = currentHeight - oldHeight;
                            var shapeCountDifference = shapeCount - oldShapeCount;

                            var amount = (rocks - shapeCount) / shapeCountDifference;

                            shapeCount += amount * shapeCountDifference;
                            extraHeight += amount * heightDifference;

                            enablePatterns = false;
                        }
                    }
                    else
                    {
                        patterns.Add(key);

                        //NOTE: Reset the pattern count
                        patternCount = 0;
                    }
                }

                while (true)
                {
                    var left = instructions.Dequeue() == '<';
                    instructionCount++;

                    if (instructions.Count == 0)
                    {
                        instructionCount = 0;
                        instructions = new Queue<char>(lines);
                    }

                    //Logger.Debug(left ? "Left" : "Right");

                    if (left && tetris.CanMoveToLeft())
                    {
                        tetris.MoveLeft();
                    }
                    else if (!left && tetris.CanMoveToRight())
                    {
                        tetris.MoveRight();
                    }

                    //Move down
                    if (tetris.CanMoveDown())
                    {
                        //Logger.Debug("Down");

                        tetris.MoveDown();
                        //tetris.DrawField(true);
                    }
                    else
                    {
                        //Logger.Debug("Rest");

                        tetris.PlaceShape();
                        //tetris.DrawField(false);

                        break;
                    }
                }
            }

            return tetris.Grid.Count - tetris.LastCollisionYIndex + extraHeight;
        }
    }
}
