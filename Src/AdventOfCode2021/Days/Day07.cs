using System;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

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

            Logger.Info($"Day 7A: {leastFuel}");
        }

        public static void StartA2()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                .SelectMany(x => x.Split(",").Select(int.Parse))
                .ToList();

            int median = lines.OrderBy(x => x).ToList()[lines.Count / 2];
            int leastFuel = lines.Sum(x => Math.Abs(x - median));

            Logger.Info($"Day 7A: {leastFuel}");
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

            Logger.Info($"Day 7B: {leastFuel}");
        }

        public static void StartB2()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                .SelectMany(x => x.Split(",").Select(int.Parse))
                .ToList();

            float average = lines.Sum(x => x) / (float)lines.Count;
            int lowAverage = (int)Math.Floor(average);
            int highAverage = (int)Math.Ceiling(average);

            int leastFuelLow = lines.Sum(x =>
            {
                int fuel = Math.Abs(x - lowAverage);

                return fuel * (fuel + 1) / 2;
            });

            int leastFuelHigh = lines.Sum(x =>
            {
                int fuel = Math.Abs(x - highAverage);

                return fuel * (fuel + 1) / 2;
            });

            Logger.Info($"Day 7B: {Math.Min(leastFuelLow, leastFuelHigh)}");
        }

        public static void StartB3()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                .SelectMany(x => x.Split(",").Select(int.Parse))
                .ToList();

            float average = lines.Sum(x => x) / (float)lines.Count;
            float adjustAverageBy = (lines.Count - 2 * lines.Count(x => x < average)) / (float)(2 * lines.Count);
            int adjustedAverage = (int) Math.Round(average + adjustAverageBy);

            int leastFuel = lines.Sum(x =>
            {
                int fuel = Math.Abs(x - adjustedAverage);

                return fuel * (fuel + 1) / 2;
            });

            Logger.Info($"Day 7B: {leastFuel}");
        }
    }
}
