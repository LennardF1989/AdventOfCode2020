using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day04
    {
        public class Board
        {
            private int[,] _grid;
            private bool[,] _marked;

            public Board(string board)
            {
                ParseBoard(board);
            }

            private void ParseBoard(string board)
            {
                var lines = board.Split("\r\n");
                var tempLine = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                _grid = new int[lines.Length, tempLine.Length];
                _marked = new bool[lines.Length, tempLine.Length];

                for (var row = 0; row < lines.Length; row++)
                {
                    var line1 = lines[row];
                    var line2 = line1
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList();

                    for (int column = 0; column < line2.Count; column++)
                    {
                        _grid[row, column] = line2[column];
                    }
                }
            }

            public void Mark(int number)
            {
                for (int row = 0; row < _grid.GetLength(0); row++)
                {
                    for (int column = 0; column < _grid.GetLength(1); column++)
                    {
                        if (_grid[row, column] != number)
                        {
                            continue;
                        }

                        _marked[row, column] = true;

                        break;
                    }
                }
            }

            public bool HasBingo()
            {
                //Left to Right
                for (int row = 0; row < _marked.GetLength(0); row++)
                {
                    bool bingo = true;
                    
                    for (int column = 0; column < _marked.GetLength(1); column++)
                    {
                        if (_marked[row, column])
                        {
                            continue;
                        }

                        bingo = false;
                        break;
                    }

                    if (bingo)
                    {
                        return true;
                    }
                }

                //Top to bottom
                for (int column = 0; column < _marked.GetLength(1); column++)
                {
                    bool bingo = true;
                    
                    for (int row = 0; row < _marked.GetLength(0); row++)
                    {
                        if (_marked[row, column])
                        {
                            continue;
                        }

                        bingo = false;
                        break;
                    }

                    if (bingo)
                    {
                        return true;
                    }
                }

                return false;
            }

            public List<int> GetUnmarkedNumbers()
            {
                List<int> unmarkedNumbers = new List<int>();

                for (int row = 0; row < _marked.GetLength(0); row++)
                {
                    for (int column = 0; column < _marked.GetLength(1); column++)
                    {
                        if (_marked[row, column])
                        {
                            continue;
                        }

                        unmarkedNumbers.Add(_grid[row, column]);
                    }
                }

                return unmarkedNumbers;
            }
        }

        public static void StartA()
        {
            var lines = File
                //.ReadAllText("Content\\Day04_Test.txt")
                .ReadAllText("Content\\Day04.txt")
                .Split("\r\n\r\n")
                ;

            var bingoOrder = lines[0].Split(",").Select(int.Parse).ToList();
            var tempBingoBoards = lines.Skip(1).ToList();
            var bingoBoards = new List<Board>();

            foreach (var bingoBoard in tempBingoBoards)
            {
                bingoBoards.Add(new Board(bingoBoard));
            }

            int lastNumber = 0;
            Board winningBoard = null;
            foreach (var number in bingoOrder)
            {
                lastNumber = number;

                foreach (var board in bingoBoards)
                {
                    board.Mark(number);

                    if (board.HasBingo())
                    {
                        winningBoard = board;

                        break;
                    }
                }

                if (winningBoard != null)
                {
                    break;
                }
            }

            int sum = winningBoard
                .GetUnmarkedNumbers()
                .Sum();

            Logger.Info($"Answer 4A: {sum * lastNumber}");
        }

        public static void StartB()
        {
            var lines = File
                    //.ReadAllText("Content\\Day04_Test.txt")
                    .ReadAllText("Content\\Day04.txt")
                    .Split("\r\n\r\n")
                ;

            var bingoOrder = lines[0].Split(",").Select(int.Parse).ToList();
            var tempBingoBoards = lines.Skip(1).ToList();
            var bingoBoards = new List<Board>();

            foreach (var bingoBoard in tempBingoBoards)
            {
                bingoBoards.Add(new Board(bingoBoard));
            }

            int lastNumber = 0;
            Board winningBoard = null;
            foreach (var number in bingoOrder)
            {
                lastNumber = number;

                List<Board> winningBoardsThisRound = new List<Board>();

                foreach (var board in bingoBoards)
                {
                    board.Mark(number);

                    if (board.HasBingo())
                    {
                        winningBoardsThisRound.Add(board);
                    }
                }

                if (winningBoardsThisRound.Count == 0)
                {
                    continue;
                }

                if (bingoBoards.Count > 1)
                {
                    winningBoardsThisRound.ForEach(x => bingoBoards.Remove(x));
                }
                else
                {
                    winningBoard = winningBoardsThisRound[0];

                    break;
                }
            }

            int sum = winningBoard
                .GetUnmarkedNumbers()
                .Sum();

            Logger.Info($"Answer 4B: {sum * lastNumber}");
        }
    }
}
