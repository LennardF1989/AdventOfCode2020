namespace AdventOfCode2024.Days
{
    public class Day01 : IDayA, IDayB
    {
        public void StartA()
        {
            var input = ParseInput();

            input.left.Sort();
            input.right.Sort();

            var answer = input.left
                .Select((x, i) => Math.Abs(input.right[i] - x))
                .Sum();

            Logger.Info($"Day 1A: {answer}");
        }

        public void StartB()
        {
            var input = ParseInput();

            var optimizedRight = input.right
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            var totalDistance = 0L;

            foreach (var leftValue in input.left)
            {
                var distance = leftValue * optimizedRight.GetValueOrDefault(leftValue, 0);

                totalDistance += distance;
            }

            var answer = totalDistance;

            Logger.Info($"Day 1B: {answer}");
        }

        private static (List<long> left, List<long> right) ParseInput()
        {
            var lines = File
                //.ReadAllLines("Content\\Day01_Test.txt")
                .ReadAllLines("Content\\Day01.txt")
                .Select(x =>
                {
                    var split = x.Split(" ", true, true);

                    return (split[0].ToLong(), split[1].ToLong());
                });

            var left = new List<long>();
            var right = new List<long>();

            foreach (var line in lines)
            {
                left.Add(line.Item1);
                right.Add(line.Item2);
            }

            return (left, right);
        }
    }
}