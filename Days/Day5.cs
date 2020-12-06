using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Days
{
    public static class Day5
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day5_Test.txt");
            var lines = File.ReadAllLines("Content\\Day5.txt");

            int highestSeatId = 0;

            foreach (string boardingpass in lines)
            {
                int min = 0;
                int max = 127;

                for (int i = 0; i < 7; i++)
                {
                    char letter = boardingpass[i];

                    if (letter == 'F')
                    {
                        //Lower-half
                        max -= (int) Math.Ceiling((max - min) / 2f);
                    }
                    else if(letter == 'B')
                    {
                        //Upper-half
                        min += (int) Math.Ceiling((max - min) / 2f);
                    }
                }

                int resultRow = min;

                min = 0;
                max = 7;
                for (int i = 7; i < 10; i++)
                {
                    char letter = boardingpass[i];

                    if (letter == 'L')
                    {
                        //Lower-half
                        max -= (int) Math.Ceiling((max - min) / 2f);
                    }
                    else if(letter == 'R')
                    {
                        //Upper-half
                        min += (int) Math.Ceiling((max - min) / 2f);
                    }
                }

                int resultColumn = min;

                int seatId = resultRow * 8 + resultColumn;

                Logger.Debug($"R:{resultRow} C:{resultColumn} S:{seatId}");

                if (seatId > highestSeatId)
                {
                    highestSeatId = seatId;
                }
            }

            Logger.Info($"Day 5A: {highestSeatId}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day5_Test.txt");
            var lines = File.ReadAllLines("Content\\Day5.txt");

            List<int> seatIds = new List<int>();

            foreach (string boardingpass in lines)
            {
                int min = 0;
                int max = 127;

                for (int i = 0; i < 7; i++)
                {
                    char letter = boardingpass[i];

                    if (letter == 'F')
                    {
                        //Lower-half
                        max -= (int) Math.Ceiling((max - min) / 2f);
                    }
                    else if(letter == 'B')
                    {
                        //Upper-half
                        min += (int) Math.Ceiling((max - min) / 2f);
                    }
                }

                int resultRow = min;

                min = 0;
                max = 7;
                for (int i = 7; i < 10; i++)
                {
                    char letter = boardingpass[i];

                    if (letter == 'L')
                    {
                        //Lower-half
                        max -= (int) Math.Ceiling((max - min) / 2f);
                    }
                    else if(letter == 'R')
                    {
                        //Upper-half
                        min += (int) Math.Ceiling((max - min) / 2f);
                    }
                }

                int resultColumn = min;

                int seatId = resultRow * 8 + resultColumn;

                Logger.Debug($"R:{resultRow} C:{resultColumn} S:{seatId}");

                seatIds.Add(seatId);
            }

            seatIds.Sort();

            int foundSeatId = 0;
            for (int i = 0; i < seatIds.Count - 1; i++)
            {
                if (seatIds[i] + 2 == seatIds[i + 1])
                {
                    foundSeatId = seatIds[i] + 1;

                    break;
                }
            }

            Logger.Info($"Day 5B: {foundSeatId}");
        }
    }
}
