using System;
using AdventOfCode2021.Days;

namespace AdventOfCode2021
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;

            //Leaderboard.Start();

            Day01.StartA();
            Day01.StartB();

            Day02.StartA();
            Day02.StartB();

            Logger.ShowDebug = true;

            Day03.StartA();
            Day03.StartB();

            Console.ReadKey();
        }
    }
}