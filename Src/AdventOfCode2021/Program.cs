using System;
using AdventOfCode.Shared;
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

            Day07.StartA();
            Day07.StartA2();
            Day07.StartB();
            Day07.StartB2();
            Day07.StartB3();
            
            Day08.StartA();
            Day08.StartB();

            Day09.StartA();
            Day09.StartB();

            Logger.ShowDebug = true;

            Day10.StartA();
            Day10.StartB();

            Console.ReadKey();
        }
    }
}