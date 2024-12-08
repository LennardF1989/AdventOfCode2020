namespace AdventOfCode2024.Days
{
    public class Template : IDayA, IDayB
    {
        public void StartA()
        {
            var lines = File
                .ReadAllLines("Content\\Day00_Test.txt")
                //.ReadAllLines("Content\\Day00.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 0A: {answer}");
        }

        public void StartB()
        {
            var lines = File
                .ReadAllLines("Content\\Day00_Test.txt")
                //.ReadAllLines("Content\\Day00.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 0B: {answer}");
        }
    }
}
