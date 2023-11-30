using System.IO;
using AdventOfCode.Shared;

namespace AdventOfCode2023.Days
{
    public static class Template
    {
        public static void StartA()
        {
            var lines = File
                .ReadAllLines("Content\\Day_Test.txt")
                //.ReadAllLines("Content\\Day.txt")
                ;

            var answer = 0;

            Logger.Info($"Day A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                .ReadAllLines("Content\\Day_Test.txt")
                //.ReadAllLines("Content\\Day.txt")
                ;

            var answer = 0;

            Logger.Info($"Day B: {answer}");
        }
    }
}
