using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Days
{
    public static class Day06
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day06_Test.txt");
            var lines = File.ReadAllLines("Content\\Day06.txt");

            StringBuilder stringBuilder = new StringBuilder();

            int total = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (!string.IsNullOrEmpty(line))
                {
                    stringBuilder.Append(line);

                    //NOTE: Don't continue on EOF
                    if (i < lines.Length - 1)
                    {
                        continue;
                    }
                }

                var allAnswers = stringBuilder.ToString();
                stringBuilder.Clear();

                int sum = allAnswers
                    .Select(x => x)
                    .Distinct()
                    .Sum(x => 1);

                Logger.Debug($"{allAnswers} => {sum}");

                total += sum;
            }

            Logger.Info($"Day 6A: {total}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day06_Test.txt");
            var lines = File.ReadAllLines("Content\\Day06.txt");

            StringBuilder stringBuilder = new StringBuilder();

            int total = 0;
            int groupSize = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (!string.IsNullOrEmpty(line))
                {
                    stringBuilder.Append(line);

                    groupSize++;

                    //NOTE: Don't continue on EOF
                    if (i < lines.Length - 1)
                    {
                        continue;
                    }
                }

                var allAnswers = stringBuilder.ToString();
                stringBuilder.Clear();

                int currentGroupSize = groupSize;
                groupSize = 0;

                var groupAnswers= allAnswers
                    .Select(x => x)
                    .GroupBy(x => x)
                    .ToDictionary(
                        x => x.Key, 
                        x => x.Sum(y => 1)
                    );

                int groupTotal = 0;
                foreach (var groupAnswer in groupAnswers)
                {
                    if (groupAnswer.Value == currentGroupSize)
                    {
                        groupTotal++;
                    }
                }

                total += groupTotal;
            }

            Logger.Info($"Day 6B: {total}");
        }
    }
}
