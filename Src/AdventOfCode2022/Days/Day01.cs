using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day01
    {
        public static void Start()
        {
            var lines = File
                //.ReadAllLines("Content\\Day01_Test.txt")
                .ReadAllLines("Content\\Day01.txt")
                ;

            var elfs = new List<int>();
            var currentTotal = 0;

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    elfs.Add(currentTotal);
                    currentTotal = 0;

                    continue;
                }

                var value = int.Parse(line);

                currentTotal += value;
            }

            elfs.Add(currentTotal);

            var answer = elfs.Max();

            Logger.Info($"Day 1A: {answer}");

            var topThree = elfs.OrderByDescending(x => x).Take(3).Sum();

            Logger.Info($"Day 1B: {topThree}");
        }
    }
}
