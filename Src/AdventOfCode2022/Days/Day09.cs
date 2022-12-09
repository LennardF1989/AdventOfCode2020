using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day09
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day09_Test.txt")
                .ReadAllLines("Content\\Day09.txt")
                .Select(ParseInput)
                .ToList();

            var answer = SimulateKnots(lines, 2);

            Logger.Info($"Day 9A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day09_Test.txt")
                //.ReadAllLines("Content\\Day09_Test2.txt")
                .ReadAllLines("Content\\Day09.txt")
                .Select(ParseInput)
                .ToList();

            var answer = SimulateKnots(lines, 10);

            Logger.Info($"Day 9B: {answer}");
        }

        private static (char Direction, int Amount) ParseInput(string x)
        {
            var splitted = x.Split(" ");

            return (Direction: splitted[0][0], Amount: int.Parse(splitted[1]));
        }

        private static int SimulateKnots(List<(char Direction, int Amount)> lines, int amountOfKnots)
        {
            var knots = new List<(int x, int y)>();

            for (int i = 0; i < amountOfKnots; i++)
            {
                knots.Add((0, 0));
            }

            var tailPositions = new HashSet<(int x, int y)>();

            foreach (var line in lines)
            {
                for (int i = 0; i < line.Amount; i++)
                {
                    HeadMove(knots, 0, line.Direction);

                    for (int i2 = 0; i2 < knots.Count - 1; i2++)
                    {
                        TailMove(knots, i2 + 1, i2);
                    }

                    tailPositions.Add(knots[^1]);
                }
            }
            
            return tailPositions.Count;
        }

        private static void HeadMove(List<(int x, int y)> knots, int headIndex, char direction)
        {
            int headX = knots[headIndex].x;
            int headY = knots[headIndex].y;

            switch (direction)
            {
                case 'U':
                    headY++;
                    break;
                case 'D':
                    headY--;
                    break;
                case 'L':
                    headX--;
                    break;
                case 'R':
                    headX++;
                    break;
            }

            knots[headIndex] = (headX, headY);
        }

        private static void TailMove(List<(int x, int y)> knots, int tailIndex, int headIndex)
        {
            var headX = knots[headIndex].x;
            var headY = knots[headIndex].y;

            var tailX = knots[tailIndex].x;
            var tailY = knots[tailIndex].y;

            var distanceX = Math.Abs(headX - tailX);
            var distanceY = Math.Abs(headY - tailY);

            if (
                (distanceX == 0 && distanceY == 0) || //Overlap
                (distanceX == 1 && distanceY == 0) || //Touching
                (distanceX == 0 && distanceY == 1) ||
                (distanceX == 1 && distanceY == 1)
            )
            {
                return;
            }

            if (distanceY == 0 && tailX > headX)
            {
                tailX--;
            }
            else if (distanceY == 0 && tailX < headX)
            {
                tailX++;
            }
            else if (distanceX == 0 && tailY > headY)
            {
                tailY--;
            }
            else if (distanceX == 0 && tailY < headY)
            {
                tailY++;
            }
            else
            {
                //Diagonal jump
                if (tailX < headX && tailY < headY)
                {
                    tailX++;
                    tailY++;
                }
                else if (tailX > headX && tailY < headY)
                {
                    tailX--;
                    tailY++;
                }
                else if (tailX < headX && tailY > headY)
                {
                    tailX++;
                    tailY--;
                }
                else if (tailX > headX && tailY > headY)
                {
                    tailX--;
                    tailY--;
                }
            }

            knots[tailIndex] = (tailX, tailY);
        }
    }
}
