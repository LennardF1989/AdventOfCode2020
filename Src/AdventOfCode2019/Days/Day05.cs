using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2019.Days
{
    public static class Day05
    {
        public static void StartA()
        {
            var lines = File
                    .ReadAllText("Content\\Day05.txt")
                    .Split(",")
                    .Select(int.Parse)
                    .ToArray()
                ;

            IntcodeComputer.RunCode(lines, 1, out var answer);

            Logger.Info($"Day 5A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                    //.ReadAllText("Content\\Day05_Test.txt")
                    .ReadAllText("Content\\Day05.txt")
                    .Split(",")
                    .Select(int.Parse)
                    .ToArray()
                ;

            //IntcodeComputer.RunCode(lines, 7, out var answer);
            //IntcodeComputer.RunCode(lines, 8, out var answer);
            //IntcodeComputer.RunCode(lines, 9, out var answer);

            IntcodeComputer.RunCode(lines, 5, out var answer);

            Logger.Info($"Day 5B: {answer}");
        }
    }
}
