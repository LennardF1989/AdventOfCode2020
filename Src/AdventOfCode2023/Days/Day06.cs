using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Extensions;

namespace AdventOfCode2023.Days
{
    public static class Day06
    {
        public record Input(long time, long distance);

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day06_Test.txt")
                .ReadAllLines("Content\\Day06.txt")
                .SelectList(x => x
                    .Split(":", true)[1]
                    .Split(" ", true, true)
                    .SelectList(y => y.ToInteger())
                );

            var inputs = lines[0]
                .Select((x, i) => new Input(x, lines[1][i]))
                .ToList();

            var answer = CalculateAnswer(inputs);

            Logger.Info($"Day 6A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day06_Test.txt")
                .ReadAllLines("Content\\Day06.txt")
                .SelectList(x => x
                    .Split(":", true)[1]
                    .Replace(" ", string.Empty)
                    .ToLong()
                );

            var inputs = new List<Input>
            {
                new(lines[0], lines[1])
            };

            var answer = CalculateAnswer(inputs);

            Logger.Info($"Day 6B: {answer}");
        }

        private static long CalculateAnswer(List<Input> inputs)
        {
            var answer = 1;

            foreach (var input in inputs)
            {
                var totalPossibilities = 0;

                for (var startTime = 0; startTime <= input.time; startTime++)
                {
                    var distance = (input.time - startTime) * startTime;

                    if (distance > input.distance)
                    {
                        totalPossibilities++;
                    }
                }

                answer *= totalPossibilities;
            }

            return answer;
        }
    }
}
