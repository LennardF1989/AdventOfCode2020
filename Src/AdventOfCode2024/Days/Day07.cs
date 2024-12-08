namespace AdventOfCode2024.Days
{
    public class Day07 : IDayA, IDayB
    {
        public void StartA()
        {
            var lines = File
                .ReadAllLines("Content\\Day07_Test.txt")
                //.ReadAllLines("Content\\Day07.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 7A: {answer}");
        }

        public void StartB()
        {
            var lines = File
                .ReadAllLines("Content\\Day07_Test.txt")
                //.ReadAllLines("Content\\Day07.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 7B: {answer}");
        }
    }
}
