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

            //if(false)
            {
                Day01.Start();
            }

            Logger.ShowDebug = true;

            Day02.StartA();
            Day02.StartB();

            Console.ReadKey();
        }
    }
}