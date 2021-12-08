using System;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2019.Days
{
    public static class Day01
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day01_Test.txt")
                .ReadAllLines("Content\\Day01.txt")
                .Select(int.Parse)
                .ToList();

            int answer = lines
                .Select(e => (int)Math.Floor(e / 3f) - 2)
                .Sum(x => x);

            Logger.Info($"Day 1A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day01_Test.txt")
                .ReadAllLines("Content\\Day01.txt")
                .Select(int.Parse)
                .ToList();

            int answer = lines.Select(e =>
            {
                int total = 0;
                int f = e;

                while(f > 0)
                {
                    total += f;
                    f = (int)Math.Floor(f / 3f) - 2;
                }

                total -= e;

                Logger.Debug(total);

                return total;
            }).Sum(x => x);

            Logger.Info($"Day 1B: {answer}");
        }
    }
}
