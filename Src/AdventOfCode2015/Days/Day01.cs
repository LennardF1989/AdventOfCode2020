using System.IO;

namespace AdventOfCode2015.Days
{
    public static class Day01
    {
        public static void StartA()
        {
            var text = File.ReadAllText("Content\\Day01.txt");

            int floor = 0;
            foreach (char c in text)
            {
                if (c == '(')
                {
                    floor++;
                }
                else if (c == ')')
                {
                    floor--;
                }
            }

            Logger.Info($"Day 1A: {floor}");
        }

        public static void StartB()
        {
            var text = File.ReadAllText("Content\\Day01.txt");

            int answer = 0;

            int floor = 0;
            for (var i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '(')
                {
                    floor++;
                }
                else if (c == ')')
                {
                    floor--;
                }

                if (floor == -1)
                {
                    answer = i + 1;

                    break;
                }
            }

            Logger.Info($"Day 1B: {answer}");
        }
    }
}
