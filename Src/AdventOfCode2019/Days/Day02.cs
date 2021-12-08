using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2019.Days
{
    public static class Day02
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllText("Content\\Day02_Test.txt")
                .ReadAllText("Content\\Day02.txt")
                .Split(",")
                .Select(int.Parse)
                .ToArray()
                ;

            lines[1] = 12;
            lines[2] = 2;

            IntcodeComputer.RunCode(lines);

            int answer = lines[0];

            Logger.Info($"Day 2A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                    //.ReadAllText("Content\\Day02_Test.txt")
                    .ReadAllText("Content\\Day02.txt")
                    .Split(",")
                    .Select(int.Parse)
                    .ToArray();

            int noun = 0;
            int verb = 0;

            for (noun = 0; noun <= 99; noun++)
            {
                for (verb = 0; verb <= 99; verb++)
                {
                    var instructions = lines.ToArray();
                    instructions[1] = noun;
                    instructions[2] = verb;

                    IntcodeComputer.RunCode(instructions);

                    if (instructions[0] == 19690720)
                    {
                        goto answer;
                    }
                }
            }

        answer:
            int answer = 100 * noun + verb;

            Logger.Info($"Day 2B: {answer}");
        }
    }
}
