using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day09
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day09_Test.txt");
            var lines = File.ReadAllLines("Content\\Day09.txt");

            var numbers = lines
                .Select(long.Parse)
                .ToList();

            //var answer = GetSequenceNumber(numbers, 5);
            var answer = GetSequenceNumber(numbers, 25);

            Logger.Info($"Day 9A: {answer}");
        }

        private static long GetSequenceNumber(List<long> numbers, int preambleSize)
        {
            for (var i = 0; i < numbers.Count; i++)
            {
                var preambles = numbers
                    .Skip(i)
                    .Take(preambleSize)
                    .ToList();

                var targetNumber = numbers[i + preambleSize];

                var result = FindSumOfTwo(preambles, targetNumber);

                if (!result)
                {
                    return targetNumber;
                }
            }

            return 0;
        }

        private static bool FindSumOfTwo(List<long> numbers, long targetNumber)
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int i2 = 0; i2 < numbers.Count; i2++)
                {
                    if (i == i2)
                    {
                        continue;
                    }

                    if (numbers[i] + numbers[i2] == targetNumber)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day09_Test.txt");
            var lines = File.ReadAllLines("Content\\Day09.txt");

            var numbers = lines
                .Select(long.Parse)
                .ToList();

            //var answer = FindSumOfList(numbers, 127);
            var answer = FindSumOfList(numbers, 373803594);

            Logger.Info($"Day 9B: {answer}");
        }

        private static long FindSumOfList(List<long> numbers, long targetNumber)
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int i2 = 0; i2 < numbers.Count; i2++)
                {
                    if (i == i2)
                    {
                        continue;
                    }

                    var contigiousNumbers = numbers
                        .Skip(i)
                        .Take(i2)
                        .ToList();

                    var sum = contigiousNumbers.Sum();

                    if (sum == targetNumber)
                    {
                        return contigiousNumbers.Min() + contigiousNumbers.Max();
                    }

                    if (sum > targetNumber)
                    {
                        break;
                    }
                }
            }

            return 0;
        }
    }
}
