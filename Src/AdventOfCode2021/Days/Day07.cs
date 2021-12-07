using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Days
{
    public static class Day07
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                .SelectMany(x => x.Split(",").Select(int.Parse))
                .ToList();

            int simulations = lines.Max();

            int leastFuel = int.MaxValue;

            for (int s = 1; s < simulations; s++)
            {
                int totalFuel = 0;

                foreach (var c in lines)
                {
                    int fuel = Math.Abs(c - s);

                    totalFuel += fuel;

                    if (totalFuel > leastFuel)
                    {
                        break;
                    }
                }

                if (totalFuel < leastFuel)
                {
                    Logger.Debug($"{s}: {totalFuel}");
                    
                    leastFuel = totalFuel;
                }
            }

            Logger.Info($"Answer 7A: {leastFuel}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                .SelectMany(x => x.Split(",").Select(int.Parse))
                .ToList();

            int simulations = lines.Max();

            int leastFuel = int.MaxValue;

            for (int s = 1; s < simulations; s++)
            {
                int totalFuel = 0;

                foreach (var c in lines)
                {
                    int fuel = Math.Abs(c - s);

                    /*int actualFuel = 0;

                    for (int f = 1; f <= fuel; f++)
                    {
                        actualFuel += f;
                    }

                    totalFuel += actualFuel;*/

                    totalFuel += fuel * (fuel + 1) / 2;

                    if (totalFuel > leastFuel)
                    {
                        Logger.Debug($"{s}: Aborted");

                        break;
                    }
                }

                if (totalFuel < leastFuel)
                {
                    Logger.Debug($"{s}: {totalFuel}");

                    leastFuel = totalFuel;
                }
            }

            Logger.Info($"Answer 7B: {leastFuel}");
        }
    }
}
