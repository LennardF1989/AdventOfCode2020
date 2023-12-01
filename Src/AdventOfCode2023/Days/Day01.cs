using System;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2023.Days
{
    public static class Day01
    {
        private static readonly string[] _words = {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        };

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day01_Test.txt")
                .ReadAllLines("Content\\Day01.txt")
                .Select(x =>
                {
                    var numbers = x
                        .Where(char.IsNumber)
                        .Select(y => (int)char.GetNumericValue(y))
                        .ToList();

                    var total = numbers[0] * 10 + numbers[^1];

                    return total;
                })
                .ToList();

            var answer = lines.Sum();

            Logger.Info($"Day 1A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day01_Test2.txt") 
                .ReadAllLines("Content\\Day01.txt")
                .Select(x =>
                {
                    var numberLeft = GetNumber(x, true);
                    var numberRight = GetNumber(x, false);

                    var total = numberLeft * 10 + numberRight;

                    return total;
                })
                .ToList();

            var answer = lines.Sum();

            Logger.Info($"Day 1B: {answer}");
        }

        private static int GetNumber(string line, bool leftToRight)
        {
            for (var i = 0; i < line.Length; i++)
            {
                var j = leftToRight ? i : line.Length - i - 1;

                if (char.IsNumber(line[j]))
                {
                    return (int)char.GetNumericValue(line[j]);
                }

                for (var w = 0; w < _words.Length; w++)
                {
                    if (line.IndexOf(_words[w], j) != j)
                    {
                        continue;
                    }

                    return w + 1;
                }
            }

            throw new NotImplementedException("This is not supposed to happen!");
        }
    }
}
