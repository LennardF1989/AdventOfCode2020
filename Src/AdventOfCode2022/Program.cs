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
            }

            Logger.ShowDebug = true;

            Day04.StartA();
            Day04.StartB();

            Console.ReadKey();
        }
    }
}