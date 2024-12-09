namespace AdventOfCode2024.Days
{
    public class Day09 : IDayA, IDayB
    {
        public void StartA()
        {
            var lines = File
                .ReadAllLines("Content\\Day09_Test.txt")
                //.ReadAllLines("Content\\Day09.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 9A: {answer}");
        }

        public void StartB()
        {
            var lines = File
                .ReadAllLines("Content\\Day09_Test.txt")
                //.ReadAllLines("Content\\Day09.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 9B: {answer}");
        }
    }
}
