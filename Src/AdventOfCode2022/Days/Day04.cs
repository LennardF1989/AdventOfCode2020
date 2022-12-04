using System;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day04
    {
        public static void StartA()
        {
            var answer = File
                //.ReadAllLines("Content\\Day04_Test.txt")
                .ReadAllLines("Content\\Day04.txt")
                .Select(GetPairs)
                .Count(pairs => 
                    (pairs.minA <= pairs.minB && pairs.maxA >= pairs.maxB) || 
                    (pairs.minB <= pairs.minA && pairs.maxB >= pairs.maxA)
                )
                ;

            Logger.Info($"Day 4A: {answer}");
        }

        public static void StartB()
        {
            var answer = File
                //.ReadAllLines("Content\\Day04_Test.txt")
                .ReadAllLines("Content\\Day04.txt")
                .Select(GetPairs)
                .Count(pairs =>
                    Math.Max(pairs.minA, pairs.minB) <= Math.Min(pairs.maxA, pairs.maxB)
                );

            Logger.Info($"Day 4B: {answer}");
        }

        private static (int minA, int maxA, int minB, int maxB) GetPairs(string pair)
        {
            var pairs = pair.Split(",");

            var pairA = pairs[0].Split("-");
            var minA = int.Parse(pairA[0]);
            var maxA = int.Parse(pairA[1]);

            var pairB = pairs[1].Split("-");
            var minB = int.Parse(pairB[0]);
            var maxB = int.Parse(pairB[1]);

            return (minA, maxA, minB, maxB);
        }
    }
}
