using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day02
    {
        private const int RockScore = 1;
        private const int PaperScore = 2;
        private const int ScissorScore = 3;

        private const int LoseScore = 0;
        private const int DrawScore = 3;
        private const int WinScore = 6;

        public static void StartA()
        {
            var answer = File
                //.ReadAllLines("Content\\Day02_Test.txt")
                .ReadAllLines("Content\\Day02.txt")
                .Select(x =>
                {
                    var result = x.Split(" ");

                    return GetScore(result[0], result[1]);
                })
                .Sum();

            Logger.Info($"Day 2A: {answer}");
        }

        public static void StartB()
        {
            var answer = File
                //.ReadAllLines("Content\\Day02_Test.txt")
                .ReadAllLines("Content\\Day02.txt")
                .Select(x =>
                {
                    var result = x.Split(" ");

                    return GetScore2(result[0], result[1]);
                })
                .Sum();

            Logger.Info($"Day 2B: {answer}");
        }

        private static int GetScore(string left, string right)
        {
            return left switch
            {
                //Rock + Rock
                "A" when right == "X" => RockScore + DrawScore,
                //Rock + Paper
                "A" when right == "Y" => PaperScore + WinScore,
                //Rock + Scissor
                "A" when right == "Z" => ScissorScore + LoseScore,
                //Paper + Rock
                "B" when right == "X" => RockScore + LoseScore,
                //Paper + Paper
                "B" when right == "Y" => PaperScore + DrawScore,
                //Paper + Scissor
                "B" when right == "Z" => ScissorScore + WinScore,
                //Scissor + Rock
                "C" when right == "X" => RockScore + WinScore,
                //Scissor + Paper
                "C" when right == "Y" => PaperScore + LoseScore,
                //Scissor + Scissor
                "C" when right == "Z" => ScissorScore + DrawScore,
                _ => 0
            };
        }

        private static int GetScore2(string left, string right)
        {
            return left switch
            {
                //Rock => Lose (Scissor)
                "A" when right == "X" => ScissorScore + LoseScore,
                //Rock => Draw (Rock)
                "A" when right == "Y" => RockScore + DrawScore,
                //Rock => Win (Paper)
                "A" when right == "Z" => PaperScore + WinScore,
                //Paper => Lose (Rock)
                "B" when right == "X" => RockScore + LoseScore,
                //Paper => Draw (Paper)
                "B" when right == "Y" => PaperScore + DrawScore,
                //Paper => Win (Scissor)
                "B" when right == "Z" => ScissorScore + WinScore,
                //Scissor => Lose (Paper)
                "C" when right == "X" => PaperScore + LoseScore,
                //Scissor => Draw (Scissor)
                "C" when right == "Y" => ScissorScore + DrawScore,
                //Scissor => Win (Rock)
                "C" when right == "Z" => RockScore + WinScore,
                _ => 0
            };
        }
    }
}
