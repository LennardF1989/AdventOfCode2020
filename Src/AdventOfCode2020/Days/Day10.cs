using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day10
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day10_Test1.txt");
            //var lines = File.ReadAllLines("Content\\Day10_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day10.txt");

            List<int> numbers = lines.Select(int.Parse).ToList();

            int startJolt = 0;
            int[] differences = new int[3];

            while (numbers.Any())
            {
                var choice = numbers
                    .Where(x => x >= startJolt - 3 && x <= startJolt + 3)
                    .OrderBy(x => x)
                    .FirstOrDefault();

                if (choice > 0)
                {
                    int difference = Math.Abs(choice - startJolt);
                    differences[difference - 1]++;

                    numbers.Remove(choice);

                    startJolt = choice;
                }
            }

            int finalJolt = startJolt + 3;
            differences[2]++;

            Logger.Debug($"Final Jolt: {finalJolt}");

            var answer = differences[0] * differences[2];

            Logger.Info($"Day 10A: {answer}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day10_Test1.txt");
            //var lines = File.ReadAllLines("Content\\Day10_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day10.txt");

            List<int> numbers = lines
                .Select(int.Parse)
                .ToList();

            var cache = new Dictionary<int, long>();

            //long answer = RecursiveJolts(numbers, cache, 0, 22);
            //long answer = RecursiveJolts(numbers, cache, 0, 52);
            long answer = RecursiveJolts(numbers, cache, 0, 173);

            Logger.Info($"Day 10B: {answer}");
        }

        private static long RecursiveJolts(List<int> numbers, Dictionary<int, long> cache, int startJolt, int targetJolt)
        {
            var choices = numbers
                .Where(x => x >= startJolt - 3 && x <= startJolt + 3)
                .OrderBy(x => x)
                .ToList();

            if (!choices.Any())
            {
                int finalJolt = startJolt + 3;

                return finalJolt == targetJolt ? 1 : 0;
            }

            long count = 0;

            for (var i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                var copy = numbers.ToList();

                for (int j = 0; j <= i; j++)
                {
                    copy.Remove(choices[j]);    
                }

                if (cache.ContainsKey(choice))
                {
                    Logger.Debug($"Already calculated {cache[choice]} path(s) for {choice} to {targetJolt}, skipping!");
                }
                else
                {
                    long totalPaths = RecursiveJolts(copy, cache, choice, targetJolt);

                    cache[choice] = totalPaths;
                }

                count += cache[choice];
            }

            return count;
        }
    }
}
