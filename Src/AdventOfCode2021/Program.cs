using System;
using AdventOfCode2021.Days;

namespace AdventOfCode2021
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;

            Leaderboard.Start();

            Day01.StartA();
            Day01.StartB();

            Day02.StartA();
            Day02.StartB();

            Day03.StartA();
            Day03.StartB();

            Day04.StartA();
            Day04.StartB();

            Day05.StartA();
            Day05.StartB();

            Day06.StartA();
            Day06.StartB();

            Logger.ShowDebug = true;

            Day07.StartA();
            Day07.StartB();

            Console.ReadKey();
        }
    }
}