using System;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2019.Days
{
    public static class Day04
    {
        public static void StartA()
        {
            var lines = File
                .ReadAllText("Content\\Day04.txt")
                .Split("-")
                .Select(int.Parse)
                .ToList();

            int hit = 0;

            for (int i = lines[0]; i < lines[1]; i++)
            {
                int last = GetDigit(i, 5);

                int equal = 0;

                for (int j = 1; j < 6; j++)
                {
                    int current = GetDigit(i, 5 - j);

                    if (current < last)
                    {
                        goto fail;
                    }

                    if (current == last)
                    {
                        equal++;
                    }

                    last = current;
                }

                if (equal > 0)
                {
                    hit++;
                }

                fail:;
            }

            Logger.Info($"Day 4A: {hit}");
        }

        public static void StartB()
        {
            var lines = File
                .ReadAllText("Content\\Day04.txt")
                .Split("-")
                .Select(int.Parse)
                .ToList();

            int hit = 0;

            for (int i = lines[0]; i < lines[1]; i++)
            {
                if (ValidateInput(i))
                {
                    hit++;
                }
            }

            Logger.Info($"Day 4B: {hit}");
        }

        private static bool ValidateInput(int i)
        {
            int last = GetDigit(i, 5);

            int equal = 0;
            int equalStreak = 0;

            for (int j = 1; j < 6; j++)
            {
                int current = GetDigit(i, 5 - j);

                if (current < last)
                {
                    return false;
                }

                if (current == last)
                {
                    equalStreak++;
                }
                else
                {
                    if (equalStreak == 1) //2+ doesn't count
                    {
                        equal++;
                    }

                    equalStreak = 0;
                }

                last = current;
            }

            if (equalStreak == 1)
            {
                equal++;
            }

            return equal > 0;
        }

        private static int GetDigit(int number, int position)
        {
            return (number / (int)Math.Pow(10, position)) % 10;
        }
    }
}
