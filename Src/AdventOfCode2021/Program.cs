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

            Logger.ShowDebug = true;

            Day05.StartA();
            Day05.StartB();

            Console.ReadKey();
        }
    }
}