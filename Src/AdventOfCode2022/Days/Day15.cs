using System.IO;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day15
    {
        public static void StartA()
        {
            var lines = File
                .ReadAllLines("Content\\Day15_Test.txt")
                //.ReadAllLines("Content\\Day15.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 15A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                .ReadAllLines("Content\\Day15_Test.txt")
                //.ReadAllLines("Content\\Day.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 15B: {answer}");
        }
    }
}
