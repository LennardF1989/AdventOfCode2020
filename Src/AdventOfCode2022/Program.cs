using System;
using AdventOfCode.Shared;
using AdventOfCode2022.Days;

namespace AdventOfCode2022
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;

            Leaderboard.Start();

            Logger.ShowDebug = true;

            Day01.StartA();
            Day01.StartB();

            Console.ReadKey();
        }
    }
}