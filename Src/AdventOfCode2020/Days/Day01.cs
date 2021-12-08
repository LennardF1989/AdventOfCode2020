using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day01
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day01_Test.txt");
            var lines = File.ReadAllLines("Content\\Day01.txt");

            var numbers = lines
                .Select(int.Parse)
                .OrderBy(x => x)
                .ToList();

            int result = 0;

            for (int i = 0; i < numbers.Count; i++)
            {
                int findNumber = 2020 - numbers[i];

                if (!numbers.Contains(findNumber))
                {
                    continue;
                }

                result = findNumber * numbers[i];

                Logger.Debug($"{findNumber} + {numbers[i]} = 2020 => {findNumber} x {numbers[i]} = {result}");

                break;
            }

            Logger.Info($"Day 1A: {result}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day01_Test.txt");
            var lines = File.ReadAllLines("Content\\Day01.txt");

            var numbers = lines
                .Select(int.Parse)
                .OrderBy(x => x)
                .ToList();

            int result = 0;

            for (int i = 0; i < numbers.Count; i++)
            {
                for (int i2 = i; i2 < numbers.Count; i2++)
                {
                    int findNumber = 2020 - numbers[i] - numbers[i2];

                    if (!numbers.Contains(findNumber))
                    {
                        continue;
                    }

                    result = findNumber * numbers[i] * + numbers[i2];

                    Logger.Debug($"{findNumber} + {numbers[i]} + {numbers[i2]} = 2020 => {findNumber} x {numbers[i]} x {numbers[i2]} = {result}");

                    break;
                }

                if (result > 0)
                {
                    break;
                }
            }

            Logger.Info($"Day 1B: {result}");
        }
    }
}
