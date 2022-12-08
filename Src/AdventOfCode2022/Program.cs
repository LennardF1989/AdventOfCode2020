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

            //if(false)
            {
                Leaderboard.Start();

                Day01.Start();
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
                Day07.StartB();
                Day08.StartA();
                Day08.StartB();
            }

            Logger.ShowDebug = true;

            Day09.StartA();
            Day09.StartB();

            Console.ReadKey();
        }
    }
}