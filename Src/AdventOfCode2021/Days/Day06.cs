using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Days
{
    public static class Day06
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day06_Test.txt")
                .ReadAllLines("Content\\Day06.txt")
                .SelectMany(x => x.Split(",").Select(int.Parse))
                .ToList();

            //fishSlow(lines, 18);
            //fishSlow(lines, 80);
            //var amount = fishFast(lines, 18);
            var amount = fishFast(lines, 80);

            Logger.Info($"Answer 6A: {amount}");
        }

        private static void fishSlow(List<int> lines, int days)
        {
            for (int i = 0; i < days; i++)
            {
                int newLaternfish = 0;

                for (var j = 0; j < lines.Count; j++)
                {
                    if (lines[j] == 0)
                    {
                        lines[j] = 6;
                        newLaternfish++;
                    }
                    else
                    {
                        lines[j]--;
                    }
                }

                for (int j = 0; j < newLaternfish; j++)
                {
                    lines.Add(8);
                }
            }
        }

        private static long fishFast(List<int> lines, int days)
        {
            long[] laternfish = new long[9];

            foreach (var line in lines)
            {
                laternfish[line]++;
            }

            for (int i = 0; i < days; i++)
            {
                long newLaternfish = laternfish[0];

                for (var j = 1; j < laternfish.Length; j++)
                {
                    laternfish[j - 1] = laternfish[j];
                }

                laternfish[6] += newLaternfish;
                laternfish[8] = newLaternfish;
            }

            return laternfish.Sum(x => x);
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day06_Test.txt")
                .ReadAllLines("Content\\Day06.txt")
                .SelectMany(x => x.Split(",").Select(int.Parse))
                .ToList();

            //fishSlow(lines, 256);
            var amount = fishFast(lines, 256);

            Logger.Info($"Answer 6B: {amount}");
        }
    }
}
