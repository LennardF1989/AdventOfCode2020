using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day02
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day02_Test.txt")
                .ReadAllLines("Content\\Day02.txt")
                .Select(x =>
                {
                    var result = x.Split(" ");

                    return new
                    {
                        instruction = result[0],
                        amount = int.Parse(result[1])
                    };
                });

            int horizontal = 0;
            int depth = 0;

            foreach (var line in lines)
            {
                if (line.instruction == "forward")
                {
                    horizontal += line.amount;
                }
                else if (line.instruction == "up")
                {
                    depth -= line.amount;
                }
                else if (line.instruction == "down")
                {
                    depth += line.amount;
                }
            }

            Logger.Info($"Answer 2A: {depth * horizontal}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day02_Test.txt")
                .ReadAllLines("Content\\Day02.txt")
                .Select(x =>
                {
                    var result = x.Split(" ");

                    return new
                    {
                        instruction = result[0],
                        amount = int.Parse(result[1])
                    };
                });

            int horizontal = 0;
            int depth = 0;
            int aim = 0;

            foreach (var line in lines)
            {
                if (line.instruction == "forward")
                {
                    horizontal += line.amount;
                    depth += aim * line.amount;
                }
                else if (line.instruction == "up")
                {
                    aim -= line.amount;
                }
                else if (line.instruction == "down")
                {
                    aim += line.amount;
                }
            }

            Logger.Info($"Answer 2B: {depth * horizontal}");
        }
    }
}