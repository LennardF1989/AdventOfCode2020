using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public static class Day2
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day2_Test.txt");
            var lines = File.ReadAllLines("Content\\Day2.txt");

            int valid = 0;

            foreach (var line in lines)
            {
                var splittedLine = line.Split(" ");
                var splittedPolicy = splittedLine[0].Split("-");

                var minPolicy = int.Parse(splittedPolicy[0]);
                var maxPolicy = int.Parse(splittedPolicy[1]);
                var charPolicy = splittedLine[1][0];

                var numberOfChars = splittedLine[2]
                    .Sum(x => x == charPolicy ? 1 : 0);

                if (numberOfChars >= minPolicy && numberOfChars <= maxPolicy)
                {
                    valid++;
                }
            }

            Logger.Info($"Day 2A: {valid}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day2_Test.txt");
            var lines = File.ReadAllLines("Content\\Day2.txt");

            int valid = 0;

            foreach (var line in lines)
            {
                var splittedLine = line.Split(" ");
                var splittedPolicy = splittedLine[0].Split("-");

                var firstPosition = int.Parse(splittedPolicy[0]);
                var secondPosition = int.Parse(splittedPolicy[1]);
                var charPolicy = splittedLine[1][0];

                var password = splittedLine[2];

                if (secondPosition <= password.Length)
                {
                    var test1 = password[firstPosition - 1] == charPolicy;
                    var test2 = password[secondPosition - 1] == charPolicy;

                    if ((test1 && !test2) || (!test1 && test2))
                    {
                        valid++;
                    }
                }
            }

            Logger.Info($"Day 2B: {valid}");
        }
    }
}
